using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Reviews;

public class ReviewCreateDTO
{
    [Range(1, int.MaxValue)] public int AttendeeId {get;set;}
    [Range(1, 5)] public int Rating {get;set;}
    [MaxLength(1000)] public string? Comment {get;set;}
}
