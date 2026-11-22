using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Orders;

public class OrderItemDTO
{
    [Range(1, int.MaxValue)] public int TicketTypeId { get; set; }
    [Range(1, 5)] public int Quantity { get; set; }
}
