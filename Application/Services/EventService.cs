using Application.Common;
using Application.DTOs.Events;
using Application.DTOs.TicketTypes;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.StatusCodes;
using Domain.Enums;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class EventService(IApplicationDbContext context) : IEventService
{
    private static readonly string[] SortFields = ["date", "price", "title"];

    public async Task<ServiceStatusCode<PagedResult<EventDTO>>> GetEventsAsync(EventFilterDTO f, CancellationToken ct = default)
    {
        var sortBy = f.SortBy.Trim().ToLowerInvariant();
        var sortDir = f.SortDir.Trim().ToLowerInvariant();

        if (!SortFields.Contains(sortBy))
            return ServiceStatusCode<PagedResult<EventDTO>>.BadRequest("sortBy должен быть: date, price или title.");
        if (sortDir != "asc" && sortDir != "desc")
            return ServiceStatusCode<PagedResult<EventDTO>>.BadRequest("sortDir должен быть: asc или desc.");

        var desc = sortDir == "desc";

        var query = context.Events.AsNoTracking().Where(e => e.Status == EventStatus.Published);

        if (!string.IsNullOrWhiteSpace(f.City))
        {
            var city = f.City.Trim().ToLower();
            query = query.Where(e => e.Venue.City.ToLower() == city);
        }

        if (f.CategoryId.HasValue)
        {
            var categoryId = f.CategoryId.Value;
            query = query.Where(e => e.Categories.Any(c => c.Id == categoryId));
        }

        if (f.DateFrom.HasValue)
        {
            var from = f.DateFrom.Value.AsUtc();
            query = query.Where(e => e.StartDate >= from);
        }

        if (f.DateTo.HasValue)
        {
            var to = f.DateTo.Value.AsUtc();
            query = query.Where(e => e.StartDate <= to);
        }

        if (f.MaxPrice.HasValue)
        {
            var maxPrice = f.MaxPrice.Value;
            query = query.Where(e => e.TicketTypes.Any(t => t.Price <= maxPrice));
        }

        if (!string.IsNullOrWhiteSpace(f.Search))
        {
            var search = f.Search.Trim().ToLower();
            query = query.Where(e => e.Title.ToLower().Contains(search));
        }

        IOrderedQueryable<Event> ordered = (sortBy, desc) switch
        {
            ("price", false) => query.OrderBy(e => e.TicketTypes.Min(t => (decimal?)t.Price)),
            ("price", true) => query.OrderByDescending(e => e.TicketTypes.Min(t => (decimal?)t.Price)),
            ("title", false) => query.OrderBy(e => e.Title),
            ("title", true) => query.OrderByDescending(e => e.Title),
            (_, true) => query.OrderByDescending(e => e.StartDate),
            _ => query.OrderBy(e => e.StartDate)
        };

        var result = await ordered
            .ThenBy(e => e.Id)
            .Select(EventMappings.ToDtoExpression)
            .ToPagedResultAsync(f.Page, f.PageSize, ct);

        return ServiceStatusCode<PagedResult<EventDTO>>.Ok(result);
    }

    public async Task<ServiceStatusCode<EventDetailsDTO>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var dto = await LoadDetailsAsync(id, ct);
        return dto is null
            ? ServiceStatusCode<EventDetailsDTO>.NotFound($"Событие {id} не найдено.")
            : ServiceStatusCode<EventDetailsDTO>.Ok(dto);
    }

    public async Task<ServiceStatusCode<EventDetailsDTO>> CreateAsync(EventCreateDTO dto, CancellationToken ct = default)
    {
        if (!await context.Venues.AnyAsync(v => v.Id == dto.VenueId, ct))
            return ServiceStatusCode<EventDetailsDTO>.NotFound($"Площадка {dto.VenueId} не найдена.");

        if (!await context.Organizers.AnyAsync(o => o.Id == dto.OrganizerId, ct))
            return ServiceStatusCode<EventDetailsDTO>.NotFound($"Организатор {dto.OrganizerId} не найден.");

        var (categories, error) = await ResolveCategoriesAsync(dto.CategoryIds, ct);
        if (error is not null)
            return ServiceStatusCode<EventDetailsDTO>.BadRequest(error);

        var ev = dto.ToEntity();
        foreach (var category in categories)
            ev.Categories.Add(category);

        context.Events.Add(ev);
        await context.SaveChangesAsync(ct);

        var details = await LoadDetailsAsync(ev.Id, ct);
        return ServiceStatusCode<EventDetailsDTO>.Created(details!);
    }

    public async Task<ServiceStatusCode<EventDetailsDTO>> UpdateAsync(int id, EventUpdateDTO dto, CancellationToken ct = default)
    {
        var ev = await context.Events.Include(e => e.Categories).FirstOrDefaultAsync(e => e.Id == id, ct);
        if (ev is null)
            return ServiceStatusCode<EventDetailsDTO>.NotFound($"Событие {id} не найдено.");

        if (ev.Status != EventStatus.Draft)
            return ServiceStatusCode<EventDetailsDTO>.Conflict("Редактировать можно только событие в статусе Draft.");

        if (!await context.Venues.AnyAsync(v => v.Id == dto.VenueId, ct))
            return ServiceStatusCode<EventDetailsDTO>.NotFound($"Площадка {dto.VenueId} не найдена.");

        var (categories, error) = await ResolveCategoriesAsync(dto.CategoryIds, ct);
        if (error is not null)
            return ServiceStatusCode<EventDetailsDTO>.BadRequest(error);

        dto.ApplyTo(ev);

        ev.Categories.Clear();
        foreach (var category in categories)
            ev.Categories.Add(category);

        await context.SaveChangesAsync(ct);

        var details = await LoadDetailsAsync(id, ct);
        return ServiceStatusCode<EventDetailsDTO>.Ok(details!);
    }

    public async Task<ServiceStatusCode<object>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var ev = await context.Events.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (ev is null)
            return ServiceStatusCode<object>.NotFound($"Событие {id} не найдено.");

        ev.IsDeleted = true;
        await context.SaveChangesAsync(ct);
        return ServiceStatusCode<object>.NoContent();
    }

    public async Task<ServiceStatusCode<EventDetailsDTO>> PublishAsync(int id, CancellationToken ct = default)
    {
        var ev = await context.Events.Include(e => e.TicketTypes).FirstOrDefaultAsync(e => e.Id == id, ct);
        if (ev is null)
            return ServiceStatusCode<EventDetailsDTO>.NotFound($"Событие {id} не найдено.");

        if (ev.Status != EventStatus.Draft)
            return ServiceStatusCode<EventDetailsDTO>.Conflict("Опубликовать можно только событие в статусе Draft.");

        if (ev.TicketTypes.Count == 0)
            return ServiceStatusCode<EventDetailsDTO>.Conflict("Нельзя опубликовать событие без типов билетов.");

        ev.Status = EventStatus.Published;
        await context.SaveChangesAsync(ct);

        var details = await LoadDetailsAsync(id, ct);
        return ServiceStatusCode<EventDetailsDTO>.Ok(details!);
    }

    public async Task<ServiceStatusCode<TicketTypeDTO>> AddTicketTypeAsync(int eventId, TicketTypeCreateDTO dto, CancellationToken ct = default)
    {
        var ev = await context.Events.FirstOrDefaultAsync(e => e.Id == eventId, ct);
        if (ev is null)
            return ServiceStatusCode<TicketTypeDTO>.NotFound($"Событие {eventId} не найдено.");

        if (ev.Status is EventStatus.Cancelled or EventStatus.Completed)
            return ServiceStatusCode<TicketTypeDTO>.Conflict("Нельзя добавлять билеты к отменённому или завершённому событию.");

        var ticketType = dto.ToEntity(eventId);
        context.TicketTypes.Add(ticketType);
        await context.SaveChangesAsync(ct);

        return ServiceStatusCode<TicketTypeDTO>.Created(ticketType.ToDto());
    }

    private async Task<EventDetailsDTO?> LoadDetailsAsync(int id, CancellationToken ct)
    {
        var ev = await context.Events
            .AsNoTracking()
            .AsSplitQuery()   
            .Include(e => e.Venue)
            .Include(e => e.Organizer)
            .Include(e => e.Categories)
            .Include(e => e.TicketTypes)
            .Include(e => e.Reviews)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        return ev?.ToDetailsDto();
    }

    private async Task<(List<Category> Categories, string? Error)> ResolveCategoriesAsync(List<int> ids, CancellationToken ct)
    {
        var distinctIds = ids.Distinct().ToList();
        var categories = await context.Categories.Where(c => distinctIds.Contains(c.Id)).ToListAsync(ct);

        var missing = distinctIds.Except(categories.Select(c => c.Id)).ToList();
        return missing.Count == 0
            ? (categories, null)
            : (categories, $"Категории не найдены: {string.Join(", ", missing)}.");
    }

    public async Task<ServiceStatusCode<EventDetailsDTO>> CancelAsync(int id, CancellationToken ct = default)
    {
        var ev = await context.Events.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (ev is null)
            return ServiceStatusCode<EventDetailsDTO>.NotFound($"Событие {id} не найдено.");

        if (ev.Status == EventStatus.Cancelled)
            return ServiceStatusCode<EventDetailsDTO>.Conflict("Событие уже отменено.");

        if (ev.Status == EventStatus.Completed)
            return ServiceStatusCode<EventDetailsDTO>.Conflict("Нельзя отменить завершённое событие.");

        ev.Status = EventStatus.Cancelled;

        // все оплаченные заказы на это событие получают статус Cancelled
        var paidOrders = await context.Orders
            .Where(o => o.Status == OrderStatus.Paid && o.Tickets.Any(t => t.TicketType.EventId == id))
            .ToListAsync(ct);

        foreach (var order in paidOrders)
            order.Status = OrderStatus.Cancelled;

        await context.SaveChangesAsync(ct);   // один SaveChanges = одна транзакция: событие и заказы меняются вместе

        var details = await LoadDetailsAsync(id, ct);
        return ServiceStatusCode<EventDetailsDTO>.Ok(details!);
    }
}