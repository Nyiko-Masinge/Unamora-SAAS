using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unamora.Domain.Entities.Identity;
using Unamora.Domain.Entities.Profiles;

namespace Unamora.Infrastructure.Persistence.Configurations;

public class TradespersonProfileConfiguration : IEntityTypeConfiguration<TradespersonProfile>
{
    public void Configure(EntityTypeBuilder<TradespersonProfile> builder)
    {
        builder.ToTable("TradespersonProfiles", "Profiles");
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => new { x.VerificationStatus, x.AvailabilityStatus });
        builder.Property(x => x.AverageRating).HasPrecision(3, 2);
        builder.Property(x => x.ServiceRadiusKm).HasPrecision(5, 2);
        builder.Property(x => x.HourlyRateMin).HasPrecision(18, 2);
        builder.Property(x => x.HourlyRateMax).HasPrecision(18, 2);
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", "Identity");
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.TokenHash);
        builder.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions", "Identity");
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions", "Identity");
        builder.HasKey(x => new { x.RoleId, x.PermissionId });
    }
}

public class VerificationRequestConfiguration : IEntityTypeConfiguration<Domain.Entities.Verification.VerificationRequest>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Verification.VerificationRequest> builder)
    {
        builder.ToTable("VerificationRequests", "Verification");
        builder.HasIndex(x => new { x.Status, x.SubmittedAt });
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}

public class PortfolioProjectConfiguration : IEntityTypeConfiguration<Domain.Entities.Portfolio.PortfolioProject>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Portfolio.PortfolioProject> builder)
    {
        builder.ToTable("PortfolioProjects", "Portfolio");
        builder.HasIndex(x => x.TradespersonProfileId);
    }
}

public class ServiceCategoryConfiguration : IEntityTypeConfiguration<Domain.Entities.Catalog.ServiceCategory>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Catalog.ServiceCategory> builder)
    {
        builder.ToTable("ServiceCategories", "Catalog");
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
