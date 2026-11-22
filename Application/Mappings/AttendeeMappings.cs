using System.Linq.Expressions;
using Application.DTOs.Attendees;
using Domain.Models;

namespace Application.Mappings;

public static class AttendeeMappings
{
    public static readonly Expression<Func<Attendee, AttendeeDTO>> ToDtoExpression = a => new AttendeeDTO
    {
        Id = a.Id,
        FullName = a.FullName,
        Email = a.Email,
        RegisteredAt = a.RegisteredAt,
        Profile = a.Profile == null ? null : new AttendeeProfileDTO
        {
            DateOfBirth = a.Profile.DateOfBirth,
            City = a.Profile.City,
            Bio = a.Profile.Bio
        }
    };

    private static readonly Func<Attendee, AttendeeDTO> compiled = ToDtoExpression.Compile();
    public static AttendeeDTO ToDto(this Attendee a) => compiled(a);

    public static Attendee ToEntity(this AttendeeCreateDTO d) => new()
    {
        FullName = d.FullName,
        Email = d.Email,
        RegisteredAt = DateTime.UtcNow
    };

    public static AttendeeProfileDTO ToDto(this AttendeeProfile p) => new()
    {
        DateOfBirth = p.DateOfBirth, City = p.City, Bio = p.Bio
    };

    public static void ApplyTo(this AttendeeProfileDTO d, AttendeeProfile p)
    {
        p.DateOfBirth = d.DateOfBirth?.AsUtc();
        p.City = d.City;
        p.Bio = d.Bio;
    }
}