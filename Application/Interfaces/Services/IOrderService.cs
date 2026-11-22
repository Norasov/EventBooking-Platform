using Application.DTOs.Orders;
using Application.StatusCodes;

namespace Application.Interfaces.Services;

public interface IOrderService
{
    Task<ServiceStatusCode<OrderDTO>> CreateAsync(CreateOrderDTO dto, CancellationToken ct = default);
    Task<ServiceStatusCode<OrderDTO>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceStatusCode<OrderDTO>> PayAsync(int id, CancellationToken ct = default);
    Task<ServiceStatusCode<OrderDTO>> CancelAsync(int id, CancellationToken ct = default);
}
