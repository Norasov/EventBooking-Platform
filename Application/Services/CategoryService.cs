using Application.DTOs.Categories;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.StatusCodes;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class CategoryService(IApplicationDbContext context) : ICategoryService
{
    public async Task<ServiceStatusCode<List<CategoryDTO>>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);
        return ServiceStatusCode<List<CategoryDTO>>.Ok(categories.Select(c => c.ToDto()).ToList());
    }
}
