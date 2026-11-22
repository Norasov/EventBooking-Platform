using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models;

public class Event
{
    public int Id {get;set;}

    [Required, MaxLength(100)] public string Title {get;set;} = null!;
    [MaxLength(2000)] public string? Description {get;set;}
    public DateTime StartDate {get;set;}
    public DateTime EndDate {get;set;}
    public EventStatus Status {get;set;} = EventStatus.Draft;

    public int VenueId {get;set;}
    public Venue Venue {get;set;}=null!;

    public int OrganizerId {get;set;}
    public Organizer Organizer {get;set;}=null!;

    public DateTime CreatedAt {get;set;}
    public DateTime? UpdatedAt {get;set;}
    public bool IsDeleted {get;set;}

    public ICollection<Category> Categories {get;set;} = [];
    public ICollection<TicketType> TicketTypes {get;set;}=[];
    public ICollection<Review> Reviews {get;set;}=[];
}
