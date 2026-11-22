using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class AttendeeProfile
{
    public int AttendeeId {get;set;}
    public DateTime? DateOfBirth {get;set;}
    [MaxLength(100)] public string? City {get;set;}
    [MaxLength(1000)] public string? Bio {get;set;}
    public Attendee Attendee {get;set;}=null!;
}
