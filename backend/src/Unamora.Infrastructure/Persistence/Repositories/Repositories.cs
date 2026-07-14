using Microsoft.EntityFrameworkCore;
using Unamora.Application.Common.Interfaces;
using Unamora.Domain.Entities.Catalog;
using Unamora.Domain.Entities.Identity;
using Unamora.Domain.Entities.Portfolio;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Entities.Verification;
using Unamora.Domain.Enums;

namespace Unamora.Infrastructure.Persistence.Repositories;

public class UserRepository(UnamoraDbContext context) : IUserRepository
{
    public Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task AddAsync(ApplicationUser user, CancellationToken cancellationToken = default) =>
        await context.Users.AddAsync(user, cancellationToken);
}

public class RefreshTokenRepository(UnamoraDbContext context) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.TokenHash == tokenHash && !r.IsRevoked, cancellationToken);

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default) =>
        await context.RefreshTokens.AddAsync(token, cancellationToken);

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await context.RefreshTokens.Where(r => r.UserId == userId && !r.IsRevoked).ToListAsync(cancellationToken);
        foreach (var token in tokens) { token.IsRevoked = true; token.RevokedAt = DateTime.UtcNow; }
    }
}

public class ServiceCategoryRepository(UnamoraDbContext context) : IServiceCategoryRepository
{
    public Task<IReadOnlyList<ServiceCategory>> GetAllActiveAsync(CancellationToken cancellationToken = default) =>
        context.ServiceCategories.AsNoTracking().Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<ServiceCategory>)t.Result);

    public Task<ServiceCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.ServiceCategories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task AddAsync(ServiceCategory category, CancellationToken cancellationToken = default) =>
        await context.ServiceCategories.AddAsync(category, cancellationToken);

    public void Update(ServiceCategory category) => context.ServiceCategories.Update(category);
    public void Delete(ServiceCategory category) => context.ServiceCategories.Remove(category);
}

public class SkillRepository(UnamoraDbContext context) : ISkillRepository
{
    public Task<IReadOnlyList<Skill>> GetAllActiveAsync(CancellationToken cancellationToken = default) =>
        context.Skills.AsNoTracking().Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<Skill>)t.Result);

    public Task<IReadOnlyList<Skill>> GetByCategoryIdAsync(int categoryId, CancellationToken cancellationToken = default) =>
        context.Skills.AsNoTracking().Where(s => s.ServiceCategoryId == categoryId && s.IsActive).OrderBy(s => s.Name).ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<Skill>)t.Result);

    public Task<Skill?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Skills.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task AddAsync(Skill skill, CancellationToken cancellationToken = default) =>
        await context.Skills.AddAsync(skill, cancellationToken);

    public void Update(Skill skill) => context.Skills.Update(skill);
    public void Delete(Skill skill) => context.Skills.Remove(skill);
}

public class TradeRepository(UnamoraDbContext context) : ITradeRepository
{
    public Task<IReadOnlyList<Trade>> GetAllActiveAsync(CancellationToken cancellationToken = default) =>
        context.Trades.AsNoTracking().Where(t => t.IsActive).OrderBy(t => t.DisplayOrder).ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<Trade>)t.Result);

    public Task<Trade?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Trades.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(Trade trade, CancellationToken cancellationToken = default) =>
        await context.Trades.AddAsync(trade, cancellationToken);

    public void Update(Trade trade) => context.Trades.Update(trade);
    public void Delete(Trade trade) => context.Trades.Remove(trade);
}

public class TradespersonProfileRepository(UnamoraDbContext context) : ITradespersonProfileRepository
{
    public Task<TradespersonProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.TradespersonProfiles.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

    public Task<TradespersonProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.TradespersonProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);

    public Task<TradespersonProfile?> GetFullProfileAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.TradespersonProfiles
            .Include(p => p.Skills).ThenInclude(s => s.Skill)
            .Include(p => p.Experiences)
            .Include(p => p.Licenses)
            .Include(p => p.Certificates)
            .Include(p => p.Languages)
            .Include(p => p.PortfolioProjects)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

    public async Task<IReadOnlyList<TradespersonProfile>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.TradespersonProfiles
            .Include(p => p.Skills).ThenInclude(s => s.Skill)
            .Include(p => p.Languages)
            .Where(p => !p.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(TradespersonProfile profile, CancellationToken cancellationToken = default) =>
        await context.TradespersonProfiles.AddAsync(profile, cancellationToken);

    public void Update(TradespersonProfile profile) => context.TradespersonProfiles.Update(profile);
}

public class ClientProfileRepository(UnamoraDbContext context) : IClientProfileRepository
{
    public Task<ClientProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.ClientProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);

    public Task<ClientProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.ClientProfiles.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

    public async Task AddAsync(ClientProfile profile, CancellationToken cancellationToken = default) =>
        await context.ClientProfiles.AddAsync(profile, cancellationToken);

    public void Update(ClientProfile profile) => context.ClientProfiles.Update(profile);
}


public class VerificationRepository(UnamoraDbContext context) : IVerificationRepository
{
    public Task<VerificationRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.VerificationRequests.Include(v => v.Documents).Include(v => v.TradespersonProfile)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public Task<VerificationRequest?> GetLatestByProfileIdAsync(Guid profileId, CancellationToken cancellationToken = default) =>
        context.VerificationRequests.Include(v => v.Documents)
            .Where(v => v.TradespersonProfileId == profileId)
            .OrderByDescending(v => v.SubmittedAt)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<IReadOnlyList<VerificationRequest>> GetPendingAsync(CancellationToken cancellationToken = default) =>
        context.VerificationRequests.Include(v => v.Documents).Include(v => v.TradespersonProfile)
            .Where(v => v.Status == TradespersonVerificationStatus.Submitted || v.Status == TradespersonVerificationStatus.InReview)
            .OrderBy(v => v.SubmittedAt)
            .ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<VerificationRequest>)t.Result);

    public async Task AddAsync(VerificationRequest request, CancellationToken cancellationToken = default) =>
        await context.VerificationRequests.AddAsync(request, cancellationToken);

    public void Update(VerificationRequest request) => context.VerificationRequests.Update(request);
}

public class PortfolioRepository(UnamoraDbContext context) : IPortfolioRepository
{
    public Task<PortfolioProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.PortfolioProjects.Include(p => p.Media).Include(p => p.ServiceCategory)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

    public Task<IReadOnlyList<PortfolioProject>> GetByProfileIdAsync(Guid profileId, CancellationToken cancellationToken = default) =>
        context.PortfolioProjects.Include(p => p.Media).Include(p => p.ServiceCategory)
            .Where(p => p.TradespersonProfileId == profileId && !p.IsDeleted)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<PortfolioProject>)t.Result);

    public async Task AddAsync(PortfolioProject project, CancellationToken cancellationToken = default) =>
        await context.PortfolioProjects.AddAsync(project, cancellationToken);

    public void Update(PortfolioProject project) => context.PortfolioProjects.Update(project);
    public void Delete(PortfolioProject project) { project.IsDeleted = true; project.DeletedAt = DateTime.UtcNow; }

    public Task<PortfolioMedia?> GetMediaByIdAsync(Guid mediaId, CancellationToken cancellationToken = default) =>
        context.PortfolioMedia.FirstOrDefaultAsync(m => m.Id == mediaId && !m.IsDeleted, cancellationToken);
}

public class AddressRepository(UnamoraDbContext context) : IAddressRepository
{
    public async Task<IReadOnlyList<Address>> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default) =>
        await context.Addresses.Where(x => x.ClientProfileId == clientProfileId && !x.IsDeleted).ToListAsync(cancellationToken);

    public Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Addresses.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task AddAsync(Address address, CancellationToken cancellationToken = default) =>
        await context.Addresses.AddAsync(address, cancellationToken);

    public void Update(Address address) => context.Addresses.Update(address);
    public void Delete(Address address) { address.IsDeleted = true; address.DeletedAt = DateTime.UtcNow; context.Addresses.Update(address); }
}

public class PaymentMethodRepository(UnamoraDbContext context) : IPaymentMethodRepository
{
    public async Task<IReadOnlyList<PaymentMethod>> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default) =>
        await context.PaymentMethods.Where(x => x.ClientProfileId == clientProfileId && !x.IsDeleted).ToListAsync(cancellationToken);

    public Task<PaymentMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.PaymentMethods.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task AddAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default) =>
        await context.PaymentMethods.AddAsync(paymentMethod, cancellationToken);

    public void Update(PaymentMethod paymentMethod) => context.PaymentMethods.Update(paymentMethod);

    public void Delete(PaymentMethod paymentMethod) { paymentMethod.IsDeleted = true; paymentMethod.DeletedAt = DateTime.UtcNow; context.PaymentMethods.Update(paymentMethod); }
}


public class ClientPreferenceRepository(UnamoraDbContext context) : IClientPreferenceRepository
{
    public Task<ClientPreference?> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default) =>
        context.ClientPreferences.FirstOrDefaultAsync(x => x.ClientProfileId == clientProfileId && !x.IsDeleted, cancellationToken);

    public async Task AddAsync(ClientPreference preference, CancellationToken cancellationToken = default) =>
        await context.ClientPreferences.AddAsync(preference, cancellationToken);

    public void Update(ClientPreference preference) => context.ClientPreferences.Update(preference);
}

public class SavedTradespersonRepository(UnamoraDbContext context) : ISavedTradespersonRepository
{
    public async Task<IReadOnlyList<SavedTradesperson>> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default) =>
        await context.SavedTradespersons
            .Include(x => x.TradespersonProfile)
            .Where(x => x.ClientProfileId == clientProfileId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<SavedTradesperson?> GetAsync(Guid clientProfileId, Guid tradespersonProfileId, CancellationToken cancellationToken = default) =>
        context.SavedTradespersons.FirstOrDefaultAsync(x => x.ClientProfileId == clientProfileId && x.TradespersonProfileId == tradespersonProfileId && !x.IsDeleted, cancellationToken);

    public async Task AddAsync(SavedTradesperson saved, CancellationToken cancellationToken = default) =>
        await context.SavedTradespersons.AddAsync(saved, cancellationToken);

    public void Delete(SavedTradesperson saved) { saved.IsDeleted = true; saved.DeletedAt = DateTime.UtcNow; context.SavedTradespersons.Update(saved); }
}

public class RecentlyViewedTradespersonRepository(UnamoraDbContext context) : IRecentlyViewedTradespersonRepository
{
    public async Task<IReadOnlyList<RecentlyViewedTradesperson>> GetRecentAsync(Guid clientProfileId, int count, CancellationToken cancellationToken = default) =>
        await context.RecentlyViewedTradespeople
            .Include(x => x.TradespersonProfile)
            .Where(x => x.ClientProfileId == clientProfileId && !x.IsDeleted)
            .OrderByDescending(x => x.ViewedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(RecentlyViewedTradesperson viewed, CancellationToken cancellationToken = default) =>
        await context.RecentlyViewedTradespeople.AddAsync(viewed, cancellationToken);
}

public class JobRequestRepository(UnamoraDbContext context) : IJobRequestRepository
{
    public Task<JobRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.JobRequests
            .Include(x => x.Trade)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task<IReadOnlyList<JobRequest>> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default) =>
        await context.JobRequests
            .Include(x => x.Trade)
            .Where(x => x.ClientProfileId == clientProfileId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(JobRequest request, CancellationToken cancellationToken = default) =>
        await context.JobRequests.AddAsync(request, cancellationToken);

    public void Update(JobRequest request) => context.JobRequests.Update(request);
}

public class QuoteRepository(UnamoraDbContext context) : IQuoteRepository
{
    public Task<Quote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Quotes
            .Include(x => x.JobRequest)
            .Include(x => x.TradespersonProfile)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task<IReadOnlyList<Quote>> GetByJobRequestIdAsync(Guid jobRequestId, CancellationToken cancellationToken = default) =>
        await context.Quotes
            .Include(x => x.TradespersonProfile)
            .Where(x => x.JobRequestId == jobRequestId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Quote>> GetByTradespersonProfileIdAsync(Guid tradespersonProfileId, CancellationToken cancellationToken = default) =>
        await context.Quotes
            .Include(x => x.JobRequest)
            .Where(x => x.TradespersonProfileId == tradespersonProfileId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Quote quote, CancellationToken cancellationToken = default) =>
        await context.Quotes.AddAsync(quote, cancellationToken);

    public void Update(Quote quote) => context.Quotes.Update(quote);
}

public class BookingRepository(UnamoraDbContext context) : IBookingRepository
{
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Bookings
            .Include(x => x.JobRequest).ThenInclude(jr => jr.Trade)
            .Include(x => x.ClientProfile)
            .Include(x => x.TradespersonProfile)
            .Include(x => x.TrackingState)
            .Include(x => x.EventLogs)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetByClientProfileIdAsync(Guid clientProfileId, CancellationToken cancellationToken = default) =>
        await context.Bookings
            .Include(x => x.JobRequest).ThenInclude(jr => jr.Trade)
            .Include(x => x.TradespersonProfile)
            .Where(x => x.ClientProfileId == clientProfileId && !x.IsDeleted)
            .OrderByDescending(x => x.ScheduledDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetByTradespersonProfileIdAsync(Guid tradespersonProfileId, CancellationToken cancellationToken = default) =>
        await context.Bookings
            .Include(x => x.JobRequest).ThenInclude(jr => jr.Trade)
            .Include(x => x.ClientProfile)
            .Where(x => x.TradespersonProfileId == tradespersonProfileId && !x.IsDeleted)
            .OrderByDescending(x => x.ScheduledDate)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default) =>
        await context.Bookings.AddAsync(booking, cancellationToken);

    public void Update(Booking booking) => context.Bookings.Update(booking);
}

public class JobTrackingStateRepository(UnamoraDbContext context) : IJobTrackingStateRepository
{
    public Task<JobTrackingState?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default) =>
        context.JobTrackingStates.FirstOrDefaultAsync(x => x.BookingId == bookingId && !x.IsDeleted, cancellationToken);

    public async Task AddAsync(JobTrackingState state, CancellationToken cancellationToken = default) =>
        await context.JobTrackingStates.AddAsync(state, cancellationToken);

    public void Update(JobTrackingState state) => context.JobTrackingStates.Update(state);
}

public class JobEventLogRepository(UnamoraDbContext context) : IJobEventLogRepository
{
    public async Task<IReadOnlyList<JobEventLog>> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default) =>
        await context.JobEventLogs
            .Where(x => x.BookingId == bookingId && !x.IsDeleted)
            .OrderBy(x => x.Timestamp)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(JobEventLog log, CancellationToken cancellationToken = default) =>
        await context.JobEventLogs.AddAsync(log, cancellationToken);
}

public class ReviewRepository(UnamoraDbContext context) : IReviewRepository
{
    public async Task<IReadOnlyList<Review>> GetByTradespersonProfileIdAsync(Guid tradespersonProfileId, CancellationToken cancellationToken = default) =>
        await context.Reviews
            .Include(x => x.Booking)
            .Where(x => x.Booking.TradespersonProfileId == tradespersonProfileId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default) =>
        await context.Reviews.AddAsync(review, cancellationToken);
}

