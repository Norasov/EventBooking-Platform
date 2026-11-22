using API.Extensions;
using Application.DTOs.Orders;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(IOrderService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDTO dto, CancellationToken ct)
        => this.ToActionResult(await service.CreateAsync(dto, ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
        => this.ToActionResult(await service.GetByIdAsync(id, ct));

    [HttpPost("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id, CancellationToken ct)
        => this.ToActionResult(await service.PayAsync(id, ct));

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
        => this.ToActionResult(await service.CancelAsync(id, ct));
}
