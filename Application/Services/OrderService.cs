using Application.DTOs.Orders;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.StatusCodes;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class OrderService(IApplicationDbContext context) : IOrderService
{
    private const int MaxTicketsPerEvent = 5;
    private static readonly TimeSpan PaymentWindow = TimeSpan.FromMinutes(15);

    public async Task<ServiceStatusCode<OrderDTO>> CreateAsync(CreateOrderDTO dto, CancellationToken ct = default)
    {
        if (!await context.Attendees.AnyAsync(a => a.Id == dto.AttendeeId, ct))
            return ServiceStatusCode<OrderDTO>.NotFound($"Участник {dto.AttendeeId} не найден.");

        var requested = dto.Items
            .GroupBy(i => i.TicketTypeId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

        var typeIds = requested.Keys.ToList();
        var ticketTypes = await context.TicketTypes
            .AsNoTracking()
            .Include(t => t.Event)
            .Where(t => typeIds.Contains(t.Id))
            .ToListAsync(ct);

        var missing = typeIds.Except(ticketTypes.Select(t => t.Id)).ToList();
        if (missing.Count > 0)
            return ServiceStatusCode<OrderDTO>.NotFound($"Типы билетов не найдены: {string.Join(", ", missing)}.");

        var now = DateTime.UtcNow;

        foreach (var tt in ticketTypes)
        {
            if (tt.Event.Status != EventStatus.Published)
                return ServiceStatusCode<OrderDTO>.Conflict($"Событие \"{tt.Event.Title}\" не опубликовано.");

            if (tt.Event.StartDate <= now)
                return ServiceStatusCode<OrderDTO>.Conflict($"Событие \"{tt.Event.Title}\" уже началось.");

            if (requested[tt.Id] > tt.Quantity - tt.SoldCount)
                return ServiceStatusCode<OrderDTO>.Conflict(
                    $"Недостаточно билетов \"{tt.Name}\": доступно {tt.Quantity - tt.SoldCount}.");
        }

        var eventIds = ticketTypes.Select(t => t.EventId).Distinct().ToList();
        var alreadyBought = await context.Tickets
            .AsNoTracking()
            .Where(t => t.AttendeeId == dto.AttendeeId
                        && eventIds.Contains(t.TicketType.EventId)
                        && (t.Order.Status == OrderStatus.Pending || t.Order.Status == OrderStatus.Paid))
            .GroupBy(t => t.TicketType.EventId)
            .Select(g => new { EventId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.EventId, x => x.Count, ct);

        foreach (var group in ticketTypes.GroupBy(t => t.EventId))
        {
            var wanted = group.Sum(t => requested[t.Id]);
            var bought = alreadyBought.GetValueOrDefault(group.Key);

            if (bought + wanted > MaxTicketsPerEvent)
                return ServiceStatusCode<OrderDTO>.Conflict(
                    $"На событие \"{group.First().Event.Title}\" можно купить не более {MaxTicketsPerEvent} билетов (уже куплено {bought}).");
        }

        await using var tx = await context.Database.BeginTransactionAsync(ct);

        foreach (var (typeId, qty) in requested)
        {
            var reserved = await context.TicketTypes
                .Where(t => t.Id == typeId && t.Quantity - t.SoldCount >= qty)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.SoldCount, t => t.SoldCount + qty), ct);

            if (reserved == 0)
                return ServiceStatusCode<OrderDTO>.Conflict("Билеты только что раскупили, попробуйте уменьшить количество.");
        }

        var order = new Order { AttendeeId = dto.AttendeeId, Status = OrderStatus.Pending };

        foreach (var tt in ticketTypes)
        {
            for (var i = 0; i < requested[tt.Id]; i++)
            {
                order.Tickets.Add(new Ticket
                {
                    TicketTypeId = tt.Id,
                    AttendeeId = dto.AttendeeId,
                    Price = tt.Price,        
                    PurchasedAt = now
                });
            }
        }

        order.TotalAmount = order.Tickets.Sum(t => t.Price);

        context.Orders.Add(order);
        await context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        var created = await LoadOrderAsync(order.Id, ct);
        return ServiceStatusCode<OrderDTO>.Created(created!);
    }

    public async Task<ServiceStatusCode<OrderDTO>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await LoadOrderAsync(id, ct);
        return order is null
            ? ServiceStatusCode<OrderDTO>.NotFound($"Заказ {id} не найден.")
            : ServiceStatusCode<OrderDTO>.Ok(order);
    }

    public async Task<ServiceStatusCode<OrderDTO>> PayAsync(int id, CancellationToken ct = default)
    {
        var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (order is null)
            return ServiceStatusCode<OrderDTO>.NotFound($"Заказ {id} не найден.");

        if (order.Status != OrderStatus.Pending)
            return ServiceStatusCode<OrderDTO>.Conflict($"Оплатить можно только заказ в статусе Pending (сейчас {order.Status}).");

        if (DateTime.UtcNow - order.CreatedAt > PaymentWindow)
            return ServiceStatusCode<OrderDTO>.Conflict("Время на оплату (15 минут) истекло.");

        var eventUnavailable = await context.TicketTypes
            .IgnoreQueryFilters()
            .AnyAsync(tt => tt.Tickets.Any(t => t.OrderId == id)
                            && (tt.Event.Status != EventStatus.Published || tt.Event.IsDeleted), ct);
        if (eventUnavailable)
            return ServiceStatusCode<OrderDTO>.Conflict("Событие отменено или недоступно, оплата невозможна.");

        order.Status = OrderStatus.Paid;
        order.PaidAt = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);

        var result = await LoadOrderAsync(id, ct);
        return ServiceStatusCode<OrderDTO>.Ok(result!);
    }

    public async Task<ServiceStatusCode<OrderDTO>> CancelAsync(int id, CancellationToken ct = default)
    {
        await using var tx = await context.Database.BeginTransactionAsync(ct);

        var order = await context.Orders.Include(o => o.Tickets).FirstOrDefaultAsync(o => o.Id == id, ct);
        if (order is null)
            return ServiceStatusCode<OrderDTO>.NotFound($"Заказ {id} не найден.");

        if (order.Status is OrderStatus.Cancelled or OrderStatus.Expired)
            return ServiceStatusCode<OrderDTO>.Conflict($"Заказ уже в статусе {order.Status}.");

        if (order.Tickets.Any(t => t.IsCheckedIn))
            return ServiceStatusCode<OrderDTO>.Conflict("Нельзя отменить заказ: по его билетам уже пройден check-in.");

        order.Status = OrderStatus.Cancelled;
        await ReleaseTicketsAsync(order.Tickets, ct);   
        await context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        var result = await LoadOrderAsync(id, ct);
        return ServiceStatusCode<OrderDTO>.Ok(result!);
    }

    private async Task<OrderDTO?> LoadOrderAsync(int id, CancellationToken ct)
    {
        var order = await context.Orders
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(o => o.Tickets)
                .ThenInclude(t => t.TicketType)
                    .ThenInclude(tt => tt.Event)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        return order?.ToDto();
    }

    private async Task ReleaseTicketsAsync(IEnumerable<Ticket> tickets, CancellationToken ct)
    {
        foreach (var group in tickets.GroupBy(t => t.TicketTypeId))
        {
            var typeId = group.Key;
            var count = group.Count();

            await context.TicketTypes
                .Where(t => t.Id == typeId)
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.SoldCount, t => t.SoldCount - count), ct);
        }
    }
}