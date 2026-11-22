using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class TicketType
{
    public int Id {get;set;}

    public int EventId {get;set;}
    public Event Event {get;set;}=null!;

    [Required, MaxLength(100)] public string Name {get;set;}=null!;
    public decimal Price {get;set;}
    public int Quantity {get;set;}
    public int SoldCount {get;set;}

    public ICollection<Ticket> Tickets {get;set;} = []; 
}
