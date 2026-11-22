using Application.Common;
using Application.DTOs.Attendees;
using Application.DTOs.Orders;
using Application.StatusCodes;

namespace Application.Interfaces.Services;

public interface IAttendeeService
{
    Task<ServiceStatusCode<AttendeeDTO>> CreateAsync(AttendeeCreateDTO dto, CancellationToken ct = default);
    Task<ServiceStatusCode<PagedResult<AttendeeDTO>>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    Task<ServiceStatusCode<AttendeeProfileDTO>> UpsertProfileAsync(int attendeeId, AttendeeProfileDTO dto, CancellationToken ct = default);
    Task<ServiceStatusCode<PagedResult<OrderDTO>>> GetOrdersAsync(int attendeeId, int page, int pageSize, CancellationToken ct = default);
    Task<ServiceStatusCode<List<TicketDTO>>> GetTicketsAsync(int attendeeId, CancellationToken ct = default);
}
