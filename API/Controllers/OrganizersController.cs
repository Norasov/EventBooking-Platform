using API.Extensions;
using Application.DTOs.Organizers;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("organizers")]
public class OrganizersController(IOrganizerService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(OrganizerCreateDTO dto, CancellationToken ct)
        => this.ToActionResult(await service.CreateAsync(dto, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => this.ToActionResult(await service.GetAllAsync(ct));

    [HttpGet("{id:int}/events")]
    public async Task<IActionResult> GetEvents(int id, CancellationToken ct)
        => this.ToActionResult(await service.GetEventsAsync(id, ct));

}
