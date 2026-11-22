using API.Extensions;
using Application.DTOs.Events;
using Application.DTOs.TicketTypes;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Reviews;

namespace API.Controllers;

[ApiController]
[Route("events")]
public class EventsController(IEventService service, IReviewService reviewService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] EventFilterDTO filter, CancellationToken ct)
        => this.ToActionResult(await service.GetEventsAsync(filter, ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
        => this.ToActionResult(await service.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create(EventCreateDTO dto, CancellationToken ct)
        => this.ToActionResult(await service.CreateAsync(dto, ct));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EventUpdateDTO dto, CancellationToken ct)
        => this.ToActionResult(await service.UpdateAsync(id, dto, ct));

    [HttpPost("{id:int}/publish")]
    public async Task<IActionResult> Publish(int id, CancellationToken ct)
        => this.ToActionResult(await service.PublishAsync(id, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => this.ToActionResult(await service.DeleteAsync(id, ct));

    [HttpPost("{id:int}/ticket-types")]
    public async Task<IActionResult> AddTicketType(int id, TicketTypeCreateDTO dto, CancellationToken ct)
        => this.ToActionResult(await service.AddTicketTypeAsync(id, dto, ct));

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
        => this.ToActionResult(await service.CancelAsync(id, ct));

    [HttpGet("{id:int}/reviews")]
    public async Task<IActionResult> GetReviews(
    int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    => this.ToActionResult(await reviewService.GetByEventAsync(id, page, pageSize, ct));

    [HttpPost("{id:int}/reviews")]
    public async Task<IActionResult> AddReview(int id, ReviewCreateDTO dto, CancellationToken ct)
        => this.ToActionResult(await reviewService.CreateAsync(id, dto, ct));
}
