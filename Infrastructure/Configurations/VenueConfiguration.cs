using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class VenueConfiguration:IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.HasData(
            new Venue { Id = 1, Name = "Главный зал", Address = "ул. Центральная, 1", City = "Ташкент", Capacity = 1500 },
            new Venue { Id = 2, Name = "Open Air Arena", Address = "пр. Победы, 25", City = "Алматы", Capacity = 5000 });
    }
}