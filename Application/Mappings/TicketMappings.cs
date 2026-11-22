using System.Linq.Expressions;
using Application.DTOs.Orders;
using Domain.Models;

namespace Application.Mappings;

public static class TicketMappings
{
    public static readonly Expression<Func<Ticket, TicketDTO>> ToDtoExpression = t => new TicketDTO
    {
        Code = t.Code,
        EventTitle = t.TicketType.Event.Title,
        TicketTypeName = t.TicketType.Name,
        Price = t.Price,
        IsCheckedIn = t.IsCheckedIn
    };

    private static readonly Func<Ticket, TicketDTO> compiled = ToDtoExpression.Compile();
    
    // Требует Include: TicketType → Event
    public static TicketDTO ToDto(this Ticket t) => compiled(t);
}