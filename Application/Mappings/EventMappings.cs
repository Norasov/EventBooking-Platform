using System.Linq.Expressions;
using Application.DTOs.Events;
using Domain.Enums;
using Domain.Models;

namespace Application.Mappings;

public static class EventMappings
{
    public static readonly Expression<Func<Event, EventDTO>> ToDtoExpression = e => new EventDTO
    {
        Id = e.Id,
        Title = e.Title,
        StartDate = e.StartDate,
        City = e.Venue.City,
        Status = e.Status,
        MinPrice = e.TicketTypes.Min(t => (decimal?)t.Price)
    };

    public static EventDetailsDTO ToDetailsDto(this Event e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        Status = e.Status,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
        Venue = e.Venue.ToDto(),
        OrganizerName = e.Organizer.CompanyName,
        Categories = e.Categories.Select(c => c.ToDto()).ToList(),
        TicketTypes = e.TicketTypes.Select(t => t.ToDto()).ToList(),
        AverageRating = e.Reviews.Count == 0 ? 0 : e.Reviews.Average(r => r.Rating),
        ReviewsCount = e.Reviews.Count
    };

    public static Event ToEntity(this EventCreateDTO d) => new()
    {
        Title = d.Title,
        Description = d.Description,
        StartDate = d.StartDate.AsUtc(),
        EndDate = d.EndDate.AsUtc(),
        VenueId = d.VenueId,
        OrganizerId = d.OrganizerId,
        Status = EventStatus.Draft
    };

    public static void ApplyTo(this EventUpdateDTO d, Event e)
    {
        e.Title = d.Title;
        e.Description = d.Description;
        e.StartDate = d.StartDate.AsUtc();
        e.EndDate = d.EndDate.AsUtc();
        e.VenueId = d.VenueId;
    }

}
