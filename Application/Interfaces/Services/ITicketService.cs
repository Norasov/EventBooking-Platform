using Application.DTOs.Orders;
using Application.StatusCodes;

namespace Application.Interfaces.Services;

public interface ITicketService
{
    Task<ServiceStatusCode<TicketDTO>> GetByCodeAsync(Guid code, CancellationToken ct = default);
    Task<ServiceStatusCode<TicketDTO>> CheckInAsync(Guid code, CancellationToken ct = default);
}
