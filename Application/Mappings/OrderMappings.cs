using System.Linq.Expressions;
using Application.DTOs.Orders;
using Domain.Models;

namespace Application.Mappings;

public static class OrderMappings
{
    public static readonly Expression<Func<Order, OrderDTO>> ToDtoExpression = o => new OrderDTO
    {
        Id = o.Id,
        Status = o.Status,
        TotalAmount = o.TotalAmount,
        CreatedAt = o.CreatedAt,
        Tickets = o.Tickets.Select(t => new TicketDTO
        {
            Code = t.Code,
            EventTitle = t.TicketType.Event.Title,
            TicketTypeName = t.TicketType.Name,
            Price = t.Price,
            IsCheckedIn = t.IsCheckedIn
        }).ToList()
    };

    private static readonly Func<Order, OrderDTO> compiled = ToDtoExpression.Compile();
    
    // Требует Include: Tickets → TicketType → Event
    public static OrderDTO ToDto(this Order o) => compiled(o);

}
