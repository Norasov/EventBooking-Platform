using Application.Common;
using Application.DTOs.Reviews;
using Application.StatusCodes;

namespace Application.Interfaces.Services;

public interface IReviewService
{
    Task<ServiceStatusCode<PagedResult<ReviewDTO>>> GetByEventAsync(int eventId, int page, int pageSize, CancellationToken ct = default);
    Task<ServiceStatusCode<ReviewDTO>> CreateAsync(int eventId, ReviewCreateDTO dto, CancellationToken ct = default);
}