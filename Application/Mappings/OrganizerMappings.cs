using Application.DTOs.Organizers;
using Domain.Models;

namespace Application.Mappings;

public static class OrganizerMappings
{
    public static OrganizerDTO ToDto(this Organizer o) => new()
    {
        Id = o.Id, CompanyName = o.CompanyName, ContactEmail = o.ContactEmail, Phone = o.Phone
    };

    public static Organizer ToEntity(this OrganizerCreateDTO d) => new()
    {
        CompanyName = d.CompanyName, ContactEmail = d.ContactEmail, Phone = d.Phone
    };
}
