using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnType("nvarchar")
            .HasMaxLength(180)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .HasColumnType("datetime")
            .HasDefaultValueSql("getdate()");
        
        builder.Property(x => x.IsVerified)
            .HasColumnType("bit")
            .IsRequired();

        builder.Property(x => x.Plan)
            .HasConversion<string>()
            .HasColumnType("nvarchar")
            .HasMaxLength(20);
    }
}