using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class OrderConfiguration: IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.TotalAmount).HasColumnType("decimal(12,2)");
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(o => o.Attendee).WithMany(a => a.Orders)
            .HasForeignKey(o => o.AttendeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
