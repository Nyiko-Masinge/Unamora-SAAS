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

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses", "Profiles");
        builder.Property(x => x.Latitude).HasPrecision(18, 10);
        builder.Property(x => x.Longitude).HasPrecision(18, 10);
        builder.HasOne(x => x.ClientProfile).WithMany(x => x.Addresses).HasForeignKey(x => x.ClientProfileId);
    }
}

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("PaymentMethods", "Profiles");
        builder.HasOne(x => x.ClientProfile).WithMany(x => x.PaymentMethods).HasForeignKey(x => x.ClientProfileId);
    }
}

public class ClientPreferenceConfiguration : IEntityTypeConfiguration<ClientPreference>
{
    public void Configure(EntityTypeBuilder<ClientPreference> builder)
    {
        builder.ToTable("ClientPreferences", "Profiles");
        builder.Property(x => x.MaxDistancePreferenceKm).HasPrecision(18, 2);
        builder.HasOne(x => x.ClientProfile).WithOne(x => x.Preference).HasForeignKey<ClientPreference>(x => x.ClientProfileId);
    }
}

public class SavedTradespersonConfiguration : IEntityTypeConfiguration<SavedTradesperson>
{
    public void Configure(EntityTypeBuilder<SavedTradesperson> builder)
    {
        builder.ToTable("SavedTradespersons", "Profiles");
        builder.HasIndex(x => new { x.ClientProfileId, x.TradespersonProfileId }).IsUnique();
        builder.HasOne(x => x.ClientProfile).WithMany(x => x.SavedTradespersons).HasForeignKey(x => x.ClientProfileId);
        builder.HasOne(x => x.TradespersonProfile).WithMany().HasForeignKey(x => x.TradespersonProfileId);
    }
}

public class RecentlyViewedTradespersonConfiguration : IEntityTypeConfiguration<RecentlyViewedTradesperson>
{
    public void Configure(EntityTypeBuilder<RecentlyViewedTradesperson> builder)
    {
        builder.ToTable("RecentlyViewedTradespeople", "Profiles");
        builder.HasOne(x => x.ClientProfile).WithMany(x => x.RecentlyViewed).HasForeignKey(x => x.ClientProfileId);
        builder.HasOne(x => x.TradespersonProfile).WithMany().HasForeignKey(x => x.TradespersonProfileId);
    }
}

public class NotificationSettingConfiguration : IEntityTypeConfiguration<NotificationSetting>
{
    public void Configure(EntityTypeBuilder<NotificationSetting> builder)
    {
        builder.ToTable("NotificationSettings", "Identity");
        builder.HasOne(x => x.User).WithOne().HasForeignKey<NotificationSetting>(x => x.UserId);
    }
}

public class JobRequestConfiguration : IEntityTypeConfiguration<Domain.Entities.Bookings.JobRequest>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Bookings.JobRequest> builder)
    {
        builder.ToTable("JobRequests", "Bookings");
        builder.Property(x => x.Latitude).HasPrecision(18, 10);
        builder.Property(x => x.Longitude).HasPrecision(18, 10);
        builder.Property(x => x.BudgetMax).HasPrecision(18, 2);
        builder.HasOne(x => x.ClientProfile).WithMany().HasForeignKey(x => x.ClientProfileId);
        builder.HasOne(x => x.Trade).WithMany().HasForeignKey(x => x.TradeId);
    }
}

public class QuoteConfiguration : IEntityTypeConfiguration<Domain.Entities.Bookings.Quote>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Bookings.Quote> builder)
    {
        builder.ToTable("Quotes", "Bookings");
        builder.Property(x => x.EstimatedHours).HasPrecision(18, 2);
        builder.Property(x => x.HourlyRate).HasPrecision(18, 2);
        builder.Property(x => x.MaterialsCost).HasPrecision(18, 2);
        builder.HasOne(x => x.JobRequest).WithMany(x => x.Quotes).HasForeignKey(x => x.JobRequestId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TradespersonProfile).WithMany().HasForeignKey(x => x.TradespersonProfileId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class BookingConfiguration : IEntityTypeConfiguration<Domain.Entities.Bookings.Booking>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Bookings.Booking> builder)
    {
        builder.ToTable("Bookings", "Bookings");
        builder.Property(x => x.AgreedHourlyRate).HasPrecision(18, 2);
        builder.Property(x => x.EstimatedCost).HasPrecision(18, 2);
        builder.Property(x => x.TotalPaid).HasPrecision(18, 2);
        builder.HasOne(x => x.ClientProfile).WithMany().HasForeignKey(x => x.ClientProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TradespersonProfile).WithMany().HasForeignKey(x => x.TradespersonProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.JobRequest).WithMany().HasForeignKey(x => x.JobRequestId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Quote).WithMany().HasForeignKey(x => x.QuoteId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class JobTrackingStateConfiguration : IEntityTypeConfiguration<Domain.Entities.Bookings.JobTrackingState>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Bookings.JobTrackingState> builder)
    {
        builder.ToTable("JobTrackingStates", "Bookings");
        builder.Property(x => x.CurrentLatitude).HasPrecision(18, 10);
        builder.Property(x => x.CurrentLongitude).HasPrecision(18, 10);
        builder.HasOne(x => x.Booking).WithOne(x => x.TrackingState).HasForeignKey<Domain.Entities.Bookings.JobTrackingState>(x => x.BookingId);
    }
}

public class JobEventLogConfiguration : IEntityTypeConfiguration<Domain.Entities.Bookings.JobEventLog>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Bookings.JobEventLog> builder)
    {
        builder.ToTable("JobEventLogs", "Bookings");
        builder.HasOne(x => x.Booking).WithMany(x => x.EventLogs).HasForeignKey(x => x.BookingId);
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Domain.Entities.Bookings.Review>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Bookings.Review> builder)
    {
        builder.ToTable("Reviews", "Bookings");
        builder.HasOne(x => x.Booking).WithMany(x => x.Reviews).HasForeignKey(x => x.BookingId);
    }
}

