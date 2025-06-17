using Microsoft.AspNetCore.Identity;
using UrlShortener.Domain.Enums;

namespace UrlShortener.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; }
    public bool IsVerified { get; set; }
    public UserPlan Plan { get; set; }

    public ApplicationUser()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Plan = UserPlan.Free;
    }
}