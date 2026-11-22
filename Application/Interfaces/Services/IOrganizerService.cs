using Application.DTOs.Events;
using Application.DTOs.Organizers;
using Application.StatusCodes;

namespace Application.Interfaces.Services;

public interface IOrganizerService
{
    Task<ServiceStatusCode<OrganizerDTO>> CreateAsync(OrganizerCreateDTO dto, CancellationToken ct = default);
    Task<ServiceStatusCode<List<OrganizerDTO>>> GetAllAsync(CancellationToken ct = default);
    Task<ServiceStatusCode<List<EventDTO>>> GetEventsAsync(int organizerId, CancellationToken ct = default);
}
