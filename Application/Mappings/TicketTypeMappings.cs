using Application.DTOs.TicketTypes;
using Domain.Models;

namespace Application.Mappings;

public static class TicketTypeMappings
{
    public static TicketTypeDTO ToDto(this TicketType t) => new()
    {
        Id = t.Id, EventId = t.EventId, Name = t.Name, Price = t.Price,
        Quantity = t.Quantity, SoldCount = t.SoldCount,
        Available = t.Quantity - t.SoldCount
    };

    public static TicketType ToEntity(this TicketTypeCreateDTO d, int eventId) => new()
    {
        EventId = eventId, Name = d.Name, Price = d.Price, Quantity = d.Quantity
    };
}
