using Domain.Enums;

namespace Application.DTOs.Orders;

public class OrderDTO
{
    public int Id {get;set;}
    public OrderStatus Status {get;set;}
    public decimal TotalAmount {get;set;}
    public DateTime CreatedAt {get;set;}
    public List<TicketDTO> Tickets {get;set;} = [];

}
