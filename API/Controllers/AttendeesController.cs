using API.Extensions;
using Application.DTOs.Attendees;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("attendees")]
public class AttendeesController(IAttendeeService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(AttendeeCreateDTO dto, CancellationToken ct)
        => this.ToActionResult(await service.CreateAsync(dto, ct));

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => this.ToActionResult(await service.GetPagedAsync(page, pageSize, ct));

    [HttpPut("{id:int}/profile")]
    public async Task<IActionResult> UpsertProfile(int id, AttendeeProfileDTO dto, CancellationToken ct)
        => this.ToActionResult(await service.UpsertProfileAsync(id, dto, ct));

    [HttpGet("{id:int}/orders")]
    public async Task<IActionResult> GetOrders(
        int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => this.ToActionResult(await service.GetOrdersAsync(id, page, pageSize, ct));

    [HttpGet("{id:int}/tickets")]
    public async Task<IActionResult> GetTickets(int id, CancellationToken ct)
        => this.ToActionResult(await service.GetTicketsAsync(id, ct));
}
