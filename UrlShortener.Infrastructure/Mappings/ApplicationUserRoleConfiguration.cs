using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Infrastructure.Identity;

namespace UrlShortener.Infrastructure.Mappings;

public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.ToTable("ApplicationUserRoles");

        builder.HasKey(x => new
        {
            x.UserId,
            x.RoleId
        });
    }
}