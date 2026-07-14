using MediatR;
using Unamora.Application.Common.Exceptions;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Modules.Profiles.Commands;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Enums;

namespace Unamora.Application.Modules.Profiles.Handlers;

public class CreateTradespersonProfileCommandHandler(
    ITradespersonProfileRepository repo, ICurrentUserService currentUser, IUnitOfWork uow) : IRequestHandler<CreateTradespersonProfileCommand, Guid>
{
    public async Task<Guid> Handle(CreateTradespersonProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var existing = await repo.GetByUserIdAsync(userId, cancellationToken);
        if (existing is not null) throw new ConflictException("Tradesperson profile already exists");

        var profile = new TradespersonProfile
        {
            UserId = userId,
            BusinessName = request.BusinessName,
            Headline = request.Headline,
            Bio = request.Bio,
            YearsOfExperience = request.YearsOfExperience,
            ProfileCompleteness = 20
        };
        await repo.AddAsync(profile, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return profile.Id;
    }
}

public class UpdateTradespersonProfileCommandHandler(
    ITradespersonProfileRepository repo, ICurrentUserService currentUser, IUnitOfWork uow) : IRequestHandler<UpdateTradespersonProfileCommand, bool>
{
    public async Task<bool> Handle(UpdateTradespersonProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await repo.GetByIdAsync(request.ProfileId, cancellationToken) ?? throw new NotFoundException("Profile not found");
        if (profile.UserId != currentUser.UserId) throw new ForbiddenException("Access denied");

        profile.BusinessName = request.BusinessName;
        profile.Headline = request.Headline;
        profile.Bio = request.Bio;
        profile.YearsOfExperience = request.YearsOfExperience;
        profile.ServiceRadiusKm = request.ServiceRadiusKm;
        profile.HourlyRateMin = request.HourlyRateMin;
        profile.HourlyRateMax = request.HourlyRateMax;
        profile.InsuranceProvider = request.InsuranceProvider;
        profile.InsurancePolicyNumber = request.InsurancePolicyNumber;
        profile.InsuranceExpiryDate = request.InsuranceExpiryDate;
        profile.HasPoliceClearance = request.HasPoliceClearance;
        profile.PoliceClearanceDate = request.PoliceClearanceDate;
        profile.BusinessRegistrationNumber = request.BusinessRegistrationNumber;
        profile.VatNumber = request.VatNumber;
        profile.ProfileCompleteness = CalculateCompleteness(profile);
        profile.ModifiedAt = DateTime.UtcNow;

        repo.Update(profile);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static int CalculateCompleteness(TradespersonProfile p)
    {
        var score = 0;
        if (!string.IsNullOrWhiteSpace(p.Bio)) score += 15;
        if (!string.IsNullOrWhiteSpace(p.Headline)) score += 10;
        if (!string.IsNullOrWhiteSpace(p.BusinessName)) score += 10;
        if (p.YearsOfExperience > 0) score += 10;
        if (p.Skills.Any()) score += 15;
        if (p.Experiences.Any()) score += 10;
        if (p.Licenses.Any()) score += 10;
        if (p.PortfolioProjects.Any()) score += 10;
        if (!string.IsNullOrWhiteSpace(p.InsuranceProvider)) score += 10;
        return Math.Min(score, 100);
    }
}

public class GetTradespersonProfileQueryHandler(ITradespersonProfileRepository repo) : IRequestHandler<GetTradespersonProfileQuery, TradespersonProfileDto?>
{
    public async Task<TradespersonProfileDto?> Handle(GetTradespersonProfileQuery request, CancellationToken cancellationToken)
    {
        var p = await repo.GetFullProfileAsync(request.ProfileId, cancellationToken);
        return p is null ? null : TradespersonProfileMapper.MapProfile(p);
    }
}

public class GetMyTradespersonProfileQueryHandler(ITradespersonProfileRepository repo, ICurrentUserService currentUser) : IRequestHandler<GetMyTradespersonProfileQuery, TradespersonProfileDto?>
{
    public async Task<TradespersonProfileDto?> Handle(GetMyTradespersonProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException("Not authenticated");
        var p = await repo.GetByUserIdAsync(userId, cancellationToken);
        if (p is null) return null;
        var full = await repo.GetFullProfileAsync(p.Id, cancellationToken);
        return full is null ? null : TradespersonProfileMapper.MapProfile(full);
    }
}

public class ToggleAvailabilityCommandHandler(ITradespersonProfileRepository repo, ICurrentUserService currentUser, IUnitOfWork uow) : IRequestHandler<ToggleAvailabilityCommand, bool>
{
    public async Task<bool> Handle(ToggleAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var profile = await repo.GetByIdAsync(request.ProfileId, cancellationToken) ?? throw new NotFoundException("Profile not found");
        if (profile.UserId != currentUser.UserId) throw new ForbiddenException("Access denied");
        profile.AvailabilityStatus = request.Status;
        profile.ModifiedAt = DateTime.UtcNow;
        repo.Update(profile);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public static class TradespersonProfileMapper
{
    public static TradespersonProfileDto MapProfile(TradespersonProfile p) => new()
    {
        Id = p.Id, UserId = p.UserId, BusinessName = p.BusinessName, Headline = p.Headline, Bio = p.Bio,
        YearsOfExperience = p.YearsOfExperience, VerificationStatus = p.VerificationStatus.ToString(),
        AvailabilityStatus = p.AvailabilityStatus.ToString(), AverageRating = p.AverageRating,
        ReviewCount = p.ReviewCount, CompletedJobsCount = p.CompletedJobsCount, ServiceRadiusKm = p.ServiceRadiusKm,
        HourlyRateMin = p.HourlyRateMin, HourlyRateMax = p.HourlyRateMax, InsuranceProvider = p.InsuranceProvider,
        HasPoliceClearance = p.HasPoliceClearance, BusinessRegistrationNumber = p.BusinessRegistrationNumber,
        ProfileCompleteness = p.ProfileCompleteness,
        Skills = p.Skills.Select(s => new TradespersonSkillDto { Id = s.Id, SkillId = s.SkillId, SkillName = s.Skill?.Name ?? string.Empty, ProficiencyLevel = s.ProficiencyLevel.ToString() }).ToList(),
        Experiences = p.Experiences.Select(e => new ExperienceDto { Id = e.Id, Title = e.Title, CompanyName = e.CompanyName, Description = e.Description, StartDate = e.StartDate, EndDate = e.EndDate, IsCurrent = e.IsCurrent }).ToList(),
        Licenses = p.Licenses.Select(l => new LicenseDto { Id = l.Id, LicenseName = l.LicenseName, LicenseNumber = l.LicenseNumber, IsVerified = l.IsVerified }).ToList(),
        Certificates = p.Certificates.Select(c => new CertificateDto { Id = c.Id, Name = c.Name, IssuingOrganization = c.IssuingOrganization, IsVerified = c.IsVerified }).ToList(),
        Languages = p.Languages.Select(l => new LanguageDto { Id = l.Id, LanguageCode = l.LanguageCode, LanguageName = l.LanguageName, Proficiency = l.Proficiency.ToString() }).ToList()
    };
}
