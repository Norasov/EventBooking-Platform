using Application.DTOs.Venues;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.StatusCodes;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class VenueService(IApplicationDbContext context) : IVenueService
{
    public async Task<ServiceStatusCode<VenueDTO>> CreateAsync(VenueCreateDTO dto, CancellationToken ct = default)
    {
        var venue = dto.ToEntity();
        context.Venues.Add(venue);
        await context.SaveChangesAsync(ct);
        return ServiceStatusCode<VenueDTO>.Created(venue.ToDto());
    }

    public async Task<ServiceStatusCode<object>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var venue = await context.Venues.FirstOrDefaultAsync(v => v.Id == id, ct);
        if (venue is null)
            return ServiceStatusCode<object>.NotFound($"Площадка {id} не найдена.");

        var hasEvents = await context.Events.IgnoreQueryFilters().AnyAsync(e => e.VenueId == id, ct);
        if (hasEvents)
            return ServiceStatusCode<object>.Conflict("Нельзя удалить площадку: у неё есть события.");

        context.Venues.Remove(venue);
        await context.SaveChangesAsync(ct);
        return ServiceStatusCode<object>.NoContent();

    }

    public async Task<ServiceStatusCode<List<VenueDTO>>> GetAllAsync(CancellationToken ct = default)
    {
        var venues = await context.Venues.AsNoTracking().OrderBy(v => v.Name).ToListAsync(ct);
        return ServiceStatusCode<List<VenueDTO>>.Ok(venues.Select(v => v.ToDto()).ToList());
    }
}