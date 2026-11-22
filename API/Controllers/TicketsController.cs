using API.Extensions;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("tickets")]
public class TicketsController(ITicketService service) : ControllerBase
{
    [HttpGet("{code:guid}")]
    public async Task<IActionResult> GetByCode(Guid code, CancellationToken ct)
        => this.ToActionResult(await service.GetByCodeAsync(code, ct));

    [HttpPost("{code:guid}/check-in")]
    public async Task<IActionResult> CheckIn(Guid code, CancellationToken ct)
        => this.ToActionResult(await service.CheckInAsync(code, ct));
}