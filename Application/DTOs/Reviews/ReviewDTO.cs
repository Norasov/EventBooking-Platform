namespace Application.DTOs.Reviews;

public class ReviewDTO
{
    public int Id {get;set;}
    public int EventId {get;set;}
    public int AttendeeId {get;set;}
    public string AttendeeName {get;set;} = null!;
    public int Rating {get;set;}
    public string? Comment {get;set;}
    public DateTime CreatedAt {get;set;}
}
