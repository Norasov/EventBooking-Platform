namespace Application.DTOs.Organizers;

public class OrganizerDTO
{
    public int Id {get;set;}
    public string CompanyName {get;set;} = null!;
    public string ContactEmail {get;set;} = null!;
    public string? Phone {get;set;}
}
