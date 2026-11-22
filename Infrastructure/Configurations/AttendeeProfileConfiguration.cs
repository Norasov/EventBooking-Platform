using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AttendeeProfileConfiguration:IEntityTypeConfiguration<AttendeeProfile>
{
    public void Configure(EntityTypeBuilder<AttendeeProfile> builder)
    {
        builder.HasKey(p => p.AttendeeId);

        builder.HasOne(p => p.Attendee).WithOne(a => a.Profile)
            .HasForeignKey<AttendeeProfile>(p => p.AttendeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
