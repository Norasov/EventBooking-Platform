using Application.DTOs.Reports;
using Application.StatusCodes;

namespace Application.Interfaces.Services;

public interface IReportService
{
    Task<ServiceStatusCode<EventSalesReportDTO>> GetEventSalesAsync(int eventId, CancellationToken ct = default);
    Task<ServiceStatusCode<List<TopEventDTO>>> GetTopEventsAsync(int count, CancellationToken ct = default);
}
