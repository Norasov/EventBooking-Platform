using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Review
{
    public int Id {get;set;}
    public int EventId {get;set;}
    public Event Event {get;set;}=null!;
    
    public int AttendeeId {get;set;}
    public Attendee Attendee {get;set;}=null!;
    
    [Range(1,5)] public int Rating {get;set;}
    [MaxLength(1000)] public string? Comment {get;set;}
    public DateTime CreatedAt {get;set;}
}
