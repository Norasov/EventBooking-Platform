using API.Extensions;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("reports")]
public class ReportsController(IReportService service) : ControllerBase
{
    [HttpGet("events/{id:int}/sales")]
    public async Task<IActionResult> GetEventSales(int id, CancellationToken ct)
        => this.ToActionResult(await service.GetEventSalesAsync(id, ct));

    [HttpGet("top-events")]
    public async Task<IActionResult> GetTopEvents([FromQuery] int count = 5, CancellationToken ct = default)
        => this.ToActionResult(await service.GetTopEventsAsync(count, ct));
}