using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
        public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.Property(t => t.Price).HasColumnType("decimal(10,2)");

        builder.HasOne(t => t.Order).WithMany(o => o.Tickets)
            .HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.Cascade);   

        builder.HasOne(t => t.TicketType).WithMany(tt => tt.Tickets)
            .HasForeignKey(t => t.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Attendee).WithMany(a => a.Tickets)
            .HasForeignKey(t => t.AttendeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
