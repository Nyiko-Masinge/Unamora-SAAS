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
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
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
    Task AddAsync(TradespersonProfile profile, CancellationToken cancellationToken = default);
    void Update(TradespersonProfile profile);
}

public interface IClientProfileRepository
{
    Task<ClientProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(ClientProfile profile, CancellationToken cancellationToken = default);
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
