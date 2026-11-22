using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Events;

public class EventFilterDTO
{
    public int Page {get;set;} = 1;
    public int PageSize {get;set;} = 10;

    [MaxLength(100)] public string? City {get;set;}
    public int? CategoryId {get;set;}
    public DateTime? DateFrom {get;set;}
    public DateTime? DateTo {get;set;}
    [Range(0, 99999999)] public decimal? MaxPrice {get;set;}
    [MaxLength(100)] public string? Search {get;set;}

    public string SortBy {get;set;} = "date";   
    public string SortDir {get;set;} = "asc";  
}
