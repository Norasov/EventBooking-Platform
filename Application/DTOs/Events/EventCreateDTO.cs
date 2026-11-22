using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Events;

public class EventCreateDTO : IValidatableObject
{
[Required, MaxLength(100)] public string Title {get;set;} = null!;
    [MaxLength(2000)] public string? Description {get;set;}
    public DateTime StartDate {get;set;}
    public DateTime EndDate {get;set;}
    [Range(1, int.MaxValue)] public int VenueId {get;set;}
    [Range(1, int.MaxValue)] public int OrganizerId {get;set;}
    public List<int> CategoryIds {get;set;} = [];
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate <= StartDate)
            yield return new ValidationResult("EndDate должна быть позже StartDate.", [nameof(EndDate)]);
    }
}