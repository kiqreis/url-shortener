using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Infrastructure.Identity;

namespace UrlShortener.Infrastructure.Mappings;

public class ApplicationUserMapping : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUser");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.HasIndex(x => x.NormalizedUserName).IsUnique();
        builder.HasIndex(x => x.NormalizedEmail).IsUnique();

        builder.Property(x => x.Email)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(x => x.NormalizedEmail)
            .HasMaxLength(180);

        builder.Property(x => x.UserName)
            .HasMaxLength(180);

        builder.Property(x => x.NormalizedUserName)
            .HasMaxLength(180);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.ConcurrencyStamp).IsConcurrencyToken();

        builder.HasMany<IdentityUserClaim<Guid>>()
            .WithOne()
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();

        builder.HasMany<IdentityUserLogin<Guid>>()
            .WithOne()
            .HasForeignKey(ul => ul.UserId)
            .IsRequired();

        builder.HasMany<IdentityUserToken<Guid>>()
            .WithOne()
            .HasForeignKey(ut => ut.UserId)
            .IsRequired();

        builder.HasMany<IdentityUserRole<Guid>>()
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();
    }
}