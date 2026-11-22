using System.Linq.Expressions;
using Application.DTOs.Reviews;
using Domain.Models;

namespace Application.Mappings;

public static class ReviewMappings
{
    public static readonly Expression<Func<Review, ReviewDTO>> ToDtoExpression = r => new ReviewDTO
    {
        Id = r.Id,
        EventId = r.EventId,
        AttendeeId = r.AttendeeId,
        AttendeeName = r.Attendee.FullName,
        Rating = r.Rating,
        Comment = r.Comment,
        CreatedAt = r.CreatedAt
    };

    private static readonly Func<Review, ReviewDTO> compiled = ToDtoExpression.Compile();
    
    // Требует Include: Attendee
    public static ReviewDTO ToDto(this Review r) => compiled(r);

    public static Review ToEntity(this ReviewCreateDTO d, int eventId) => new()
    {
        EventId = eventId, AttendeeId = d.AttendeeId, Rating = d.Rating, Comment = d.Comment
    };

}
