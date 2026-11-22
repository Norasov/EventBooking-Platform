using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasOne(r => r.Event).WithMany(e => e.Reviews)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Attendee).WithMany(a => a.Reviews)
            .HasForeignKey(r => r.AttendeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.EventId, r.AttendeeId }).IsUnique();
    }
}
