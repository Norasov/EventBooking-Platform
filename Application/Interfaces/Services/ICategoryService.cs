using Application.DTOs.Categories;
using Application.StatusCodes;

namespace Application.Interfaces.Services;

public interface ICategoryService
{
    Task<ServiceStatusCode<List<CategoryDTO>>> GetAllAsync(CancellationToken ct = default);
}
