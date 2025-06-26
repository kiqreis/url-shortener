using Microsoft.AspNetCore.Identity;

namespace UrlShortener.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        Id = Guid.NewGuid();
    }
}