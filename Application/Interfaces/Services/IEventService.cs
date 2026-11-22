using Application.Common;
using Application.DTOs.Events;
using Application.DTOs.TicketTypes;
using Application.StatusCodes;

namespace Application.Interfaces.Services;

public interface IEventService
{
    Task<ServiceStatusCode<PagedResult<EventDTO>>> GetEventsAsync(EventFilterDTO filter, CancellationToken ct = default);
    Task<ServiceStatusCode<EventDetailsDTO>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceStatusCode<EventDetailsDTO>> CreateAsync(EventCreateDTO dto, CancellationToken ct = default);
    Task<ServiceStatusCode<EventDetailsDTO>> UpdateAsync(int id, EventUpdateDTO dto, CancellationToken ct = default);
    Task<ServiceStatusCode<object>> DeleteAsync(int id, CancellationToken ct = default);
    Task<ServiceStatusCode<EventDetailsDTO>> PublishAsync(int id, CancellationToken ct = default);
    Task<ServiceStatusCode<TicketTypeDTO>> AddTicketTypeAsync(int eventId, TicketTypeCreateDTO dto, CancellationToken ct = default);
    Task<ServiceStatusCode<EventDetailsDTO>> CancelAsync(int id, CancellationToken ct = default);
}
