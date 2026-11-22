using Application.Common;
using Application.DTOs.Reviews;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.StatusCodes;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ReviewService(IApplicationDbContext context) : IReviewService
{
    public async Task<ServiceStatusCode<PagedResult<ReviewDTO>>> GetByEventAsync(int eventId, int page, int pageSize, CancellationToken ct = default)
    {
        if (!await context.Events.AnyAsync(e => e.Id == eventId, ct))
            return ServiceStatusCode<PagedResult<ReviewDTO>>.NotFound($"Событие {eventId} не найдено.");

        var result = await context.Reviews
            .AsNoTracking()
            .Where(r => r.EventId == eventId)
            .OrderByDescending(r => r.CreatedAt)
            .ThenBy(r => r.Id)
            .Select(ReviewMappings.ToDtoExpression)
            .ToPagedResultAsync(page, pageSize, ct);

        return ServiceStatusCode<PagedResult<ReviewDTO>>.Ok(result);
    }

    public async Task<ServiceStatusCode<ReviewDTO>> CreateAsync(int eventId, ReviewCreateDTO dto, CancellationToken ct = default)
    {
        var ev = await context.Events
            .AsNoTracking()
            .Where(e => e.Id == eventId)
            .Select(e => new { e.EndDate })
            .FirstOrDefaultAsync(ct);

        if (ev is null)
            return ServiceStatusCode<ReviewDTO>.NotFound($"Событие {eventId} не найдено.");

        if (!await context.Attendees.AnyAsync(a => a.Id == dto.AttendeeId, ct))
            return ServiceStatusCode<ReviewDTO>.NotFound($"Участник {dto.AttendeeId} не найден.");

        if (ev.EndDate > DateTime.UtcNow)
            return ServiceStatusCode<ReviewDTO>.Conflict("Отзыв можно оставить только после окончания события.");

        var attended = await context.Tickets.AnyAsync(
            t => t.AttendeeId == dto.AttendeeId && t.TicketType.EventId == eventId && t.IsCheckedIn, ct);
        if (!attended)
            return ServiceStatusCode<ReviewDTO>.Conflict("Отзыв можно оставить только при наличии билета с пройденным check-in.");

        if (await context.Reviews.AnyAsync(r => r.EventId == eventId && r.AttendeeId == dto.AttendeeId, ct))
            return ServiceStatusCode<ReviewDTO>.Conflict("Вы уже оставили отзыв на это событие.");

        var review = dto.ToEntity(eventId);
        context.Reviews.Add(review);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return ServiceStatusCode<ReviewDTO>.Conflict("Вы уже оставили отзыв на это событие.");
        }

        var created = await context.Reviews
            .AsNoTracking()
            .Where(r => r.Id == review.Id)
            .Select(ReviewMappings.ToDtoExpression)
            .FirstAsync(ct);

        return ServiceStatusCode<ReviewDTO>.Created(created);
    }
}