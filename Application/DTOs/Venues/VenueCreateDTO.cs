using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Venues;

public class VenueCreateDTO
{
    [Required, MaxLength(100)] public string Name {get;set;} = null!;
    [Required, MaxLength(200)] public string Address {get;set;} = null!;
    [Required, MaxLength(100)] public string City {get;set;} = null!;
    [Range(1, int.MaxValue)] public int Capacity {get;set;}
}
