using Unamora.Domain.Entities.Bookings;
using Unamora.Domain.Entities.Catalog;
using Unamora.Domain.Entities.Identity;
using Unamora.Domain.Entities.Portfolio;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Entities.Verification;

namespace Unamora.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(ApplicationUser user, CancellationToken cancellationToken = default);
}

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
    void RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IServiceCategoryRepository
{
    Task<IReadOnlyList<ServiceCategory>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<ServiceCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(ServiceCategory category, CancellationToken cancellationToken = default);
    void Update(ServiceCategory category);
    void Delete(ServiceCategory category);
}

public interface ISkillRepository
{
    Task<IReadOnlyList<Skill>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Skill>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<Skill?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Skill skill, CancellationToken cancellationToken = default);
    void Update(Skill skill);
    void Delete(Skill skill);
}

public interface ITradeRepository
{
    Task<IReadOnlyList<Trade>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    Task<Trade?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Trade trade, CancellationToken cancellationToken = default);
    void Update(Trade trade);
    void Delete(Trade trade);
}

public interface ITradespersonProfileRepository
{
    Task<TradespersonProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TradespersonProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TradespersonProfile?> GetFullProfileAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TradespersonProfile>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TradespersonProfile profile, CancellationToken cancellationToken = default);
    void Update(TradespersonProfile profile);
}

public interface IClientProfileRepository
{
    Task<ClientProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ClientProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(ClientProfile profile, CancellationToken cancellationToken = default);
    void Update(ClientProfile profile);
}

public interface IVerificationRepository
{
    Task<VerificationRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VerificationRequest?> GetLatestByProfileIdAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VerificationRequest>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task AddAsync(VerificationRequest request, CancellationToken cancellationToken = default);
    void Update(VerificationRequest request);
}

public interface IPortfolioRepository
{
    Task<PortfolioProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PortfolioProject>> GetByProfileIdAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task AddAsync(PortfolioProject project, CancellationToken cancellationToken = default);
    void Update(PortfolioProject project);
    void Delete(PortfolioProject project);
    Task<PortfolioMedia?> GetMediaByIdAsync(Guid mediaId, CancellationToken cancellationToken = default);
}

public interface IAddressRepository
{
    Task<IReadOnlyList<Address>> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default);
    Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Address address, CancellationToken cancellationToken = default);
    void Update(Address address);
    void Delete(Address address);
}

public interface IPaymentMethodRepository
{
    Task<IReadOnlyList<PaymentMethod>> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default);
    Task<PaymentMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
    void Update(PaymentMethod paymentMethod);
    void Delete(PaymentMethod paymentMethod);
}


public interface IClientPreferenceRepository
{
    Task<ClientPreference?> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default);
    Task AddAsync(ClientPreference preference, CancellationToken cancellationToken = default);
    void Update(ClientPreference preference);
}

public interface ISavedTradespersonRepository
{
    Task<IReadOnlyList<SavedTradesperson>> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default);
    Task<SavedTradesperson?> GetAsync(Guid clientProfileId, Guid tradespersonProfileId, CancellationToken cancellationToken = default);
    Task AddAsync(SavedTradesperson saved, CancellationToken cancellationToken = default);
    void Delete(SavedTradesperson saved);
}

public interface IRecentlyViewedTradespersonRepository
{
    Task<IReadOnlyList<RecentlyViewedTradesperson>> GetRecentAsync(Guid clientProfileId, int count, CancellationToken cancellationToken = default);
    Task AddAsync(RecentlyViewedTradesperson viewed, CancellationToken cancellationToken = default);
}

public interface IJobRequestRepository
{
    Task<JobRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobRequest>> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default);
    Task AddAsync(JobRequest request, CancellationToken cancellationToken = default);
    void Update(JobRequest request);
}

public interface IQuoteRepository
{
    Task<Quote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Quote>> GetByJobRequestIdAsync(Guid jobRequestId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Quote>> GetByTradespersonProfileIdAsync(Guid tradespersonProfileId, CancellationToken cancellationToken = default);
    Task AddAsync(Quote quote, CancellationToken cancellationToken = default);
    void Update(Quote quote);
}

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Booking>> GetByTradespersonProfileIdAsync(Guid tradespersonProfileId, CancellationToken cancellationToken = default);
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
    void Update(Booking booking);
}

public interface IJobTrackingStateRepository
{
    Task<JobTrackingState?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task AddAsync(JobTrackingState state, CancellationToken cancellationToken = default);
    void Update(JobTrackingState state);
}

public interface IJobEventLogRepository
{
    Task<IReadOnlyList<JobEventLog>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task AddAsync(JobEventLog log, CancellationToken cancellationToken = default);
}

public interface IReviewRepository
{
    Task<IReadOnlyList<Review>> GetByTradespersonProfileIdAsync(Guid tradespersonProfileId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
}

