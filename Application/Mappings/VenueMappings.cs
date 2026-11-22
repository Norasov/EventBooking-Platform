using Application.DTOs.Venues;
using Domain.Models;

namespace Application.Mappings;

public static class VenueMappings
{
    public static VenueDTO ToDto(this Venue v) => new()
    {
        Id = v.Id, Name = v.Name, Address = v.Address, City = v.City, Capacity = v.Capacity
    };

    public static Venue ToEntity(this VenueCreateDTO d) => new()
    {
        Name = d.Name, Address = d.Address, City = d.City, Capacity = d.Capacity
    };

}
