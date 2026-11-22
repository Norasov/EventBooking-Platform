using Application.DTOs.Categories;
using Domain.Models;

namespace Application.Mappings;

public static class CategoryMappings
{
    public static CategoryDTO ToDto(this Category c) => new() { Id = c.Id, Name = c.Name };
}
