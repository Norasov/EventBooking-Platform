namespace Domain.Models;

public class Ticket
{
    public int Id {get;set;}
    
    public int OrderId {get;set;}
    public Order Order {get;set;}=null!;

    public int TicketTypeId {get;set;}
    public TicketType TicketType {get;set;}=null!;

    public int AttendeeId {get;set;}
    public Attendee Attendee {get;set;}=null!;

    public Guid Code {get;set;} = Guid.NewGuid();
    public decimal Price {get;set;}
    public DateTime PurchasedAt {get;set;}
    public bool IsCheckedIn {get;set;}
    public DateTime? CheckedInAt {get;set;}
}
