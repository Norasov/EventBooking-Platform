using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.TicketTypes;

public class TicketTypeCreateDTO
{
    [Required, MaxLength(100)] public string Name {get;set;} = null!;
    [Range(0, 99999999)] public decimal Price {get;set;}
    [Range(1, int.MaxValue)] public int Quantity {get;set;}
}
