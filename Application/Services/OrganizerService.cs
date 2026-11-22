using Application.DTOs.Events;
using Application.DTOs.Organizers;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.StatusCodes;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class OrganizerService(IApplicationDbContext context) : IOrganizerService
{
    public async Task<ServiceStatusCode<OrganizerDTO>> CreateAsync(OrganizerCreateDTO dto, CancellationToken ct = default)
    {
        var organizer = dto.ToEntity();
        context.Organizers.Add(organizer);
        await context.SaveChangesAsync(ct);
        return ServiceStatusCode<OrganizerDTO>.Created(organizer.ToDto());
    }

    public async Task<ServiceStatusCode<List<OrganizerDTO>>> GetAllAsync(CancellationToken ct = default)
    {
        var organizers = await context.Organizers.AsNoTracking().OrderBy(o => o.CompanyName).ToListAsync(ct);
        return ServiceStatusCode<List<OrganizerDTO>>.Ok(organizers.Select(o => o.ToDto()).ToList());
    }

    public async Task<ServiceStatusCode<List<EventDTO>>> GetEventsAsync(int organizerId, CancellationToken ct = default)
    {
        if (!await context.Organizers.AnyAsync(o => o.Id == organizerId, ct))
            return ServiceStatusCode<List<EventDTO>>.NotFound($"Организатор {organizerId} не найден.");

        var events = await context.Events
            .AsNoTracking()
            .Where(e => e.OrganizerId == organizerId)
            .OrderBy(e => e.StartDate)
            .Select(EventMappings.ToDtoExpression)
            .ToListAsync(ct);

        return ServiceStatusCode<List<EventDTO>>.Ok(events);
    }
}
