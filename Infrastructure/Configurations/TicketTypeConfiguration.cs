using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class TicketTypeConfiguration:IEntityTypeConfiguration<TicketType>
{
    public void Configure(EntityTypeBuilder<TicketType> builder)
    {
        builder.Property(t => t.Price).HasColumnType("decimal(10,2)");

        builder.HasOne(t => t.Event).WithMany(e => e.TicketTypes)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_TicketTypes_Quantity_NonNegative", "\"Quantity\" >= 0");
            t.HasCheckConstraint("CK_TicketTypes_SoldCount_LessOrEqualQuantity", "\"SoldCount\" <= \"Quantity\"");
        });
    }
}
