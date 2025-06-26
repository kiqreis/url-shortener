using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UrlShortener.Infrastructure.Identity.Configurations;

public class ApplicationUserRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable("ApplicationUserRoleClaims");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClaimType)
            .HasMaxLength(255);

        builder.Property(x => x.ClaimValue)
            .HasMaxLength(255);
    }
}