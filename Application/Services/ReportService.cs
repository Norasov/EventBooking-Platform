using Application.DTOs.Reports;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.StatusCodes;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ReportService(IApplicationDbContext context) : IReportService
{
    public async Task<ServiceStatusCode<EventSalesReportDTO>> GetEventSalesAsync(int eventId, CancellationToken ct = default)
    {
        var ev = await context.Events
            .AsNoTracking()
            .Where(e => e.Id == eventId)
            .Select(e => new { e.Id, e.Title })
            .FirstOrDefaultAsync(ct);

        if (ev is null)
            return ServiceStatusCode<EventSalesReportDTO>.NotFound($"Событие {eventId} не найдено.");

        var sales = await context.Tickets
            .AsNoTracking()
            .Where(t => t.TicketType.EventId == eventId && t.Order.Status == OrderStatus.Paid)
            .GroupBy(t => t.TicketTypeId)
            .Select(g => new
            {
                TicketTypeId = g.Key,
                Sold = g.Count(),
                Revenue = g.Sum(t => t.Price),
                CheckedIn = g.Count(t => t.IsCheckedIn)
            })
            .ToDictionaryAsync(x => x.TicketTypeId, ct);

        var types = await context.TicketTypes
            .AsNoTracking()
            .Where(tt => tt.EventId == eventId)
            .OrderBy(tt => tt.Id)
            .Select(tt => new { tt.Id, tt.Name, tt.Price, tt.Quantity })
            .ToListAsync(ct);

        var rows = types.Select(tt =>
        {
            sales.TryGetValue(tt.Id, out var s);
            var sold = s?.Sold ?? 0;
            var checkedIn = s?.CheckedIn ?? 0;

            return new TicketTypeSalesDTO
            {
                TicketTypeId = tt.Id,
                Name = tt.Name,
                Price = tt.Price,
                Quantity = tt.Quantity,
                TicketsSold = sold,
                Revenue = s?.Revenue ?? 0m,
                FillPercent = tt.Quantity == 0 ? 0 : Math.Round(sold * 100.0 / tt.Quantity, 2),
                CheckInPercent = sold == 0 ? 0 : Math.Round(checkedIn * 100.0 / sold, 2)
            };
        }).ToList();

        var report = new EventSalesReportDTO
        {
            EventId = ev.Id,
            EventTitle = ev.Title,
            TotalTicketsSold = rows.Sum(r => r.TicketsSold),
            TotalRevenue = rows.Sum(r => r.Revenue),
            TicketTypes = rows
        };

        return ServiceStatusCode<EventSalesReportDTO>.Ok(report);
    }

    public async Task<ServiceStatusCode<List<TopEventDTO>>> GetTopEventsAsync(int count, CancellationToken ct = default)
    {
        count = Math.Clamp(count, 1, 100);

        var top = await context.Tickets
            .AsNoTracking()
            .Where(t => t.Order.Status == OrderStatus.Paid)
            .GroupBy(t => new { t.TicketType.EventId, t.TicketType.Event.Title })
            .Select(g => new TopEventDTO
            {
                EventId = g.Key.EventId,
                Title = g.Key.Title,
                TicketsSold = g.Count(),
                Revenue = g.Sum(t => t.Price)
            })
            .OrderByDescending(x => x.Revenue)
            .ThenBy(x => x.EventId)
            .Take(count)
            .ToListAsync(ct);

        return ServiceStatusCode<List<TopEventDTO>>.Ok(top);
    }
}