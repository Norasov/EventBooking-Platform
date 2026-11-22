using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Orders;

public class CreateOrderDTO
{
    [Range(1, int.MaxValue)] public int AttendeeId {get;set;}
    [Required, MinLength(1)] public List<OrderItemDTO> Items {get;set;} = [];
}
