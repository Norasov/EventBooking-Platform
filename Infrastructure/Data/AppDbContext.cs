using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext:DbContext, IApplicationDbContext
{
public AppDbContext(DbContextOptions<AppDbContext> options):base(options){}

    public DbSet<Venue> Venues {get;set;} = null!;
    public DbSet<Organizer> Organizers {get;set;} = null!;
    public DbSet<Category> Categories {get;set;} = null!;
    public DbSet<Event> Events {get;set;} = null!;
    public DbSet<TicketType> TicketTypes {get;set;} = null!;
    public DbSet<Attendee> Attendees {get;set;} = null!;
    public DbSet<AttendeeProfile> AttendeeProfiles {get;set;} = null!;
    public DbSet<Order> Orders {get;set;} = null!;
    public DbSet<Ticket> Tickets {get;set;} = null!;
    public DbSet<Review> Reviews {get;set;} = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Event ev)
            {
                if (entry.State == EntityState.Added) ev.CreatedAt = now;
                else if (entry.State == EntityState.Modified) ev.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Added)
            {
                if (entry.Entity is Order o) o.CreatedAt = now;
                if (entry.Entity is Review r) r.CreatedAt = now;
            }
        }

        return await base.SaveChangesAsync(ct);
    }
}
