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

    public async Task AddAsync(TradespersonProfile profile, CancellationToken cancellationToken = default) =>
        await context.TradespersonProfiles.AddAsync(profile, cancellationToken);

    public void Update(TradespersonProfile profile) => context.TradespersonProfiles.Update(profile);
}

public class ClientProfileRepository(UnamoraDbContext context) : IClientProfileRepository
{
    public Task<ClientProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.ClientProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);

    public async Task AddAsync(ClientProfile profile, CancellationToken cancellationToken = default) =>
        await context.ClientProfiles.AddAsync(profile, cancellationToken);
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
