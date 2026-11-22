using Application.Common;
using Application.DTOs.Attendees;
using Application.DTOs.Orders;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.StatusCodes;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class AttendeeService(IApplicationDbContext context) : IAttendeeService
{
    public async Task<ServiceStatusCode<AttendeeDTO>> CreateAsync(AttendeeCreateDTO dto, CancellationToken ct = default)
    {
        var email = dto.Email.Trim();

        if (await context.Attendees.AnyAsync(a => a.Email.ToLower() == email.ToLower(), ct))
            return ServiceStatusCode<AttendeeDTO>.Conflict($"Участник с email {email} уже существует.");

        var attendee = dto.ToEntity();
        attendee.Email = email;
        context.Attendees.Add(attendee);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return ServiceStatusCode<AttendeeDTO>.Conflict($"Участник с email {email} уже существует.");
        }

        return ServiceStatusCode<AttendeeDTO>.Created(attendee.ToDto());
    }

    public async Task<ServiceStatusCode<PagedResult<OrderDTO>>> GetOrdersAsync(int attendeeId, int page, int pageSize, CancellationToken ct = default)
    {
        if (!await context.Attendees.AnyAsync(a => a.Id == attendeeId, ct))
            return ServiceStatusCode<PagedResult<OrderDTO>>.NotFound($"Участник {attendeeId} не найден.");

        var result = await context.Orders
            .AsNoTracking()
            .Where(o => o.AttendeeId == attendeeId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(OrderMappings.ToDtoExpression)
            .ToPagedResultAsync(page, pageSize, ct);

        return ServiceStatusCode<PagedResult<OrderDTO>>.Ok(result);
    }

    public async Task<ServiceStatusCode<PagedResult<AttendeeDTO>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var result = await context.Attendees
            .AsNoTracking()
            .OrderBy(a => a.Id)
            .Select(AttendeeMappings.ToDtoExpression)
            .ToPagedResultAsync(page, pageSize, ct);

        return ServiceStatusCode<PagedResult<AttendeeDTO>>.Ok(result);
    }

    public async Task<ServiceStatusCode<List<TicketDTO>>> GetTicketsAsync(int attendeeId, CancellationToken ct = default)
    {
        if (!await context.Attendees.AnyAsync(a => a.Id == attendeeId, ct))
            return ServiceStatusCode<List<TicketDTO>>.NotFound($"Участник {attendeeId} не найден.");

        var tickets = await context.Tickets
            .AsNoTracking()
            .Where(t => t.AttendeeId == attendeeId)
            .OrderByDescending(t => t.PurchasedAt)
            .Select(TicketMappings.ToDtoExpression)
            .ToListAsync(ct);

        return ServiceStatusCode<List<TicketDTO>>.Ok(tickets);
    }

    public async Task<ServiceStatusCode<AttendeeProfileDTO>> UpsertProfileAsync(int attendeeId, AttendeeProfileDTO dto, CancellationToken ct = default)
    {
        var profile = await context.AttendeeProfiles.FirstOrDefaultAsync(p => p.AttendeeId == attendeeId, ct);

        if (profile is null)
        {
            if (!await context.Attendees.AnyAsync(a => a.Id == attendeeId, ct))
                return ServiceStatusCode<AttendeeProfileDTO>.NotFound($"Участник {attendeeId} не найден.");

            profile = new AttendeeProfile { AttendeeId = attendeeId };
            context.AttendeeProfiles.Add(profile);
        }

        dto.ApplyTo(profile);
        await context.SaveChangesAsync(ct);
        return ServiceStatusCode<AttendeeProfileDTO>.Ok(profile.ToDto());
    }
}