using Application.DTOs.Categories;
using Application.DTOs.TicketTypes;
using Application.DTOs.Venues;
using Domain.Enums;

namespace Application.DTOs.Events;

public class EventDetailsDTO
{
    public int Id {get;set;}
    public string Title {get;set;} = null!;
    public string? Description {get;set;}
    public DateTime StartDate {get;set;}
    public DateTime EndDate {get;set;}
    public EventStatus Status {get;set;}
    public DateTime CreatedAt {get;set;}
    public DateTime? UpdatedAt {get;set;}
    public VenueDTO Venue {get;set;} = null!;
    public string OrganizerName {get;set;} = null!;
    public List<CategoryDTO> Categories {get;set;} = [];
    public List<TicketTypeDTO> TicketTypes {get;set;} = [];
    public double AverageRating {get;set;}
    public int ReviewsCount {get;set;}
}
