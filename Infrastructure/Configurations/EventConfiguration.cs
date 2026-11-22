using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasOne(e => e.Venue).WithMany(v => v.Events)
            .HasForeignKey(e => e.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Organizer).WithMany(o => o.Events)
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasMany(e => e.Categories).WithMany(c => c.Events)
            .UsingEntity(j => j.ToTable("EventCategories"));

        builder.HasIndex(e => new { e.Status, e.StartDate });

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Events_EndDate_After_StartDate",
            "\"EndDate\" > \"StartDate\""));

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
