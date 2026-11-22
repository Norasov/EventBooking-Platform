using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class CategoryConfiguration:IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(c => c.Name).IsUnique();

        builder.HasData(
            new Category { Id = 1, Name = "Музыка" },
            new Category { Id = 2, Name = "Спорт" },
            new Category { Id = 3, Name = "Технологии" },
            new Category { Id = 4, Name = "Искусство" },
            new Category { Id = 5, Name = "Образование" },
            new Category { Id = 6, Name = "Бизнес" });
    }
}
