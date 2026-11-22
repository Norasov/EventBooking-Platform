namespace Application.DTOs.Attendees;

public class AttendeeDTO
{
    public int Id {get;set;}
    public string FullName {get;set;} = null!;
    public string Email {get;set;} = null!;
    public DateTime RegisteredAt {get;set;}
    public AttendeeProfileDTO? Profile {get;set;}
}
