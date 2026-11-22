using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Organizer
{
    public int Id {get;set;}
    [Required, MaxLength(100)] public string CompanyName {get;set;}=null!;
    [Required, MaxLength(150)] public string ContactEmail {get;set;}=null!;
    [MaxLength(20)] public string? Phone {get;set;}
    public ICollection<Event> Events {get;set;} = [];
}
