using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Organizers;

public class OrganizerCreateDTO
{
    [Required, MaxLength(100)] public string CompanyName {get;set;} = null!;
    [Required, EmailAddress, MaxLength(150)] public string ContactEmail {get;set;} = null!;
    [MaxLength(20)] public string? Phone {get;set;}
}
