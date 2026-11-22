using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Venue> Venues {get;}
    DbSet<Organizer> Organizers {get;}
    DbSet<Category> Categories {get;}
    DbSet<Event> Events {get;}
    DbSet<TicketType> TicketTypes {get;}
    DbSet<Attendee> Attendees {get;}
    DbSet<AttendeeProfile> AttendeeProfiles {get;}
    DbSet<Order> Orders {get;}
    DbSet<Ticket> Tickets {get;}
    DbSet<Review> Reviews {get;}

    DatabaseFacade Database {get;}
    Task<int> SaveChangesAsync(CancellationToken ct = default);

}
