namespace Application.DTOs.Orders;

public class TicketDTO
{
    public Guid Code {get;set;}
    public string EventTitle {get;set;} = null!;
    public string TicketTypeName {get;set;} = null!;
    public decimal Price {get;set;}
    public bool IsCheckedIn {get;set;}
}
