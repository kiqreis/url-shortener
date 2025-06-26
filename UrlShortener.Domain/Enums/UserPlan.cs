using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Domain.Enums;

public enum UserPlan
{
    [Display(Name = "Free Plan")]
    Free = 0,
    
    [Display(Name = "Premium Plan")]
    Pro = 1,
    
    [Display(Name = "Business Plan")]
    Business = 2
}