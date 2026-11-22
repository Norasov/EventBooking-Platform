namespace Application.DTOs.Reports;

public class EventSalesReportDTO
{
    public int EventId {get;set;}
    public string EventTitle {get;set;} = null!;
    public int TotalTicketsSold {get;set;}
    public decimal TotalRevenue {get;set;}
    public List<TicketTypeSalesDTO> TicketTypes {get;set;} = [];
}
