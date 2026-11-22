namespace Application.DTOs.Reports;

public class TopEventDTO
{
    public int EventId { get; set; }
    public string Title { get; set; } = null!;
    public int TicketsSold { get; set; }
    public decimal Revenue { get; set; }
}
