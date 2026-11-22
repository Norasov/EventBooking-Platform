using Application.DTOs.Orders;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.StatusCodes;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TicketService(IApplicationDbContext context) : ITicketService
{
    private static readonly TimeSpan CheckInWindow = TimeSpan.FromHours(2);

    public async Task<ServiceStatusCode<TicketDTO>> GetByCodeAsync(Guid code, CancellationToken ct = default)
    {
        var ticket = await context.Tickets
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(t => t.Code == code)
            .Select(TicketMappings.ToDtoExpression)
            .FirstOrDefaultAsync(ct);

        return ticket is null
            ? ServiceStatusCode<TicketDTO>.NotFound("Билет не найден.")
            : ServiceStatusCode<TicketDTO>.Ok(ticket);
    }

    public async Task<ServiceStatusCode<TicketDTO>> CheckInAsync(Guid code, CancellationToken ct = default)
    {
        var info = await context.Tickets
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(t => t.Code == code)
            .Select(t => new
            {
                t.Id,
                t.IsCheckedIn,
                OrderStatus = t.Order.Status,
                StartDate = t.TicketType.Event.StartDate
            })
            .FirstOrDefaultAsync(ct);

        if (info is null)
            return ServiceStatusCode<TicketDTO>.NotFound("Билет не найден.");

        if (info.OrderStatus != OrderStatus.Paid)
            return ServiceStatusCode<TicketDTO>.Conflict("Check-in возможен только для оплаченного билета.");

        if (info.IsCheckedIn)
            return ServiceStatusCode<TicketDTO>.Conflict("Билет уже использован.");

        if (DateTime.UtcNow < info.StartDate - CheckInWindow)
            return ServiceStatusCode<TicketDTO>.Conflict("Check-in открывается за 2 часа до начала события.");

        var updated = await context.Tickets
            .Where(t => t.Id == info.Id && !t.IsCheckedIn)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.IsCheckedIn, true)
                .SetProperty(t => t.CheckedInAt, (DateTime?)DateTime.UtcNow), ct);

        if (updated == 0)
            return ServiceStatusCode<TicketDTO>.Conflict("Билет уже использован.");

        return await GetByCodeAsync(code, ct);
    }
}