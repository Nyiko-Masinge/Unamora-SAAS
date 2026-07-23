using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unamora.Application.Common.Interfaces;
using Unamora.Domain.Entities.Identity;
using Unamora.Infrastructure.Identity;
using Unamora.Infrastructure.Persistence;
using Unamora.Infrastructure.Persistence.Repositories;
using Unamora.Infrastructure.Services;
using Unamora.Infrastructure.Storage;

namespace Unamora.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=unamora.db";
        services.AddDbContext<UnamoraDbContext>(options =>
            options.UseSqlite(connectionString,
                b => b.MigrationsAssembly(typeof(UnamoraDbContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<UnamoraDbContext>()
            .AddDefaultTokenProviders();


        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<ITradeRepository, TradeRepository>();
        services.AddScoped<ITradespersonProfileRepository, TradespersonProfileRepository>();
        services.AddScoped<IClientProfileRepository, ClientProfileRepository>();
        services.AddScoped<IVerificationRepository, VerificationRepository>();
        services.AddScoped<IPortfolioRepository, PortfolioRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
        services.AddScoped<IClientPreferenceRepository, ClientPreferenceRepository>();
        services.AddScoped<ISavedTradespersonRepository, SavedTradespersonRepository>();
        services.AddScoped<IRecentlyViewedTradespersonRepository, RecentlyViewedTradespersonRepository>();
        services.AddScoped<IJobRequestRepository, JobRequestRepository>();
        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IJobTrackingStateRepository, JobTrackingStateRepository>();
        services.AddScoped<IJobEventLogRepository, JobEventLogRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IMatchingService, MatchingService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IEscrowService, EscrowService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ICommissionService, CommissionService>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IOcrService, OcrService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddSingleton<IAiAssistantService, AiAssistantService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IFraudRiskService, FraudRiskService>();

        return services;
    }
}
