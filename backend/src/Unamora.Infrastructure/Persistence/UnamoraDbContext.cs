using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Unamora.Domain.Entities.Bookings;
using Unamora.Domain.Entities.Catalog;
using Unamora.Domain.Entities.Identity;
using Unamora.Domain.Entities.Portfolio;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Entities.Verification;

namespace Unamora.Infrastructure.Persistence;

public class UnamoraDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public UnamoraDbContext(DbContextOptions<UnamoraDbContext> options) : base(options) { }

    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
    public DbSet<PhoneVerificationToken> PhoneVerificationTokens => Set<PhoneVerificationToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<NotificationSetting> NotificationSettings => Set<NotificationSetting>();

    public DbSet<ClientProfile> ClientProfiles => Set<ClientProfile>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<ClientPreference> ClientPreferences => Set<ClientPreference>();
    public DbSet<SavedTradesperson> SavedTradespersons => Set<SavedTradesperson>();
    public DbSet<RecentlyViewedTradesperson> RecentlyViewedTradespeople => Set<RecentlyViewedTradesperson>();
    public DbSet<TradespersonProfile> TradespersonProfiles => Set<TradespersonProfile>();
    public DbSet<TradespersonSkill> TradespersonSkills => Set<TradespersonSkill>();
    public DbSet<TradespersonExperience> TradespersonExperiences => Set<TradespersonExperience>();
    public DbSet<TradespersonLicense> TradespersonLicenses => Set<TradespersonLicense>();
    public DbSet<TradespersonCertificate> TradespersonCertificates => Set<TradespersonCertificate>();
    public DbSet<TradespersonLanguage> TradespersonLanguages => Set<TradespersonLanguage>();
    public DbSet<TradespersonServiceArea> TradespersonServiceAreas => Set<TradespersonServiceArea>();
    public DbSet<TradespersonAvailability> TradespersonAvailabilities => Set<TradespersonAvailability>();
    public DbSet<ServicePackage> ServicePackages => Set<ServicePackage>();

    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Trade> Trades => Set<Trade>();

    public DbSet<VerificationRequest> VerificationRequests => Set<VerificationRequest>();
    public DbSet<VerificationDocument> VerificationDocuments => Set<VerificationDocument>();
    public DbSet<VerificationReviewLog> VerificationReviewLogs => Set<VerificationReviewLog>();

    public DbSet<PortfolioProject> PortfolioProjects => Set<PortfolioProject>();
    public DbSet<PortfolioMedia> PortfolioMedia => Set<PortfolioMedia>();

    public DbSet<JobRequest> JobRequests => Set<JobRequest>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<JobTrackingState> JobTrackingStates => Set<JobTrackingState>();
    public DbSet<JobEventLog> JobEventLogs => Set<JobEventLog>();
    public DbSet<Review> Reviews => Set<Review>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(UnamoraDbContext).Assembly);

        builder.Entity<ApplicationUser>().ToTable("Users", "Identity");
        builder.Entity<ApplicationRole>().ToTable("Roles", "Identity");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles", "Identity");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims", "Identity");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins", "Identity");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims", "Identity");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens", "Identity");
    }
}
