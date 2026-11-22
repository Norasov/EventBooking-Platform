using Application.DTOs.Venues;
using Application.StatusCodes;

namespace Application.Interfaces.Services;

public interface IVenueService
{
    Task<ServiceStatusCode<VenueDTO>> CreateAsync(VenueCreateDTO dto, CancellationToken ct = default);
    Task<ServiceStatusCode<List<VenueDTO>>> GetAllAsync(CancellationToken ct = default);
    Task<ServiceStatusCode<object>> DeleteAsync(int id, CancellationToken ct = default);
}
