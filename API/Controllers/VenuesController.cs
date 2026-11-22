using API.Extensions;
using Application.DTOs.Venues;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("venues")]
public class VenuesController(IVenueService service) : ControllerBase
{
     [HttpPost]
    public async Task<IActionResult> Create(VenueCreateDTO dto, CancellationToken ct)
        => this.ToActionResult(await service.CreateAsync(dto, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => this.ToActionResult(await service.GetAllAsync(ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => this.ToActionResult(await service.DeleteAsync(id, ct));
}
