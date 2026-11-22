namespace Application.DTOs.Reports;

public class TicketTypeSalesDTO
{
    public int TicketTypeId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int TicketsSold { get; set; }
    public decimal Revenue { get; set; }
    public double FillPercent { get; set; }
    public double CheckInPercent { get; set; }
}
