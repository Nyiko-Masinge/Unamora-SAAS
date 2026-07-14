using FluentValidation;
using MediatR;
using Unamora.Domain.Enums;

namespace Unamora.Application.Modules.Profiles.Commands;

public record CreateTradespersonProfileCommand(
    string? BusinessName,
    string? Headline,
    string? Bio,
    int YearsOfExperience) : IRequest<Guid>;

public record UpdateTradespersonProfileCommand(
    Guid ProfileId,
    string? BusinessName,
    string? Headline,
    string? Bio,
    int YearsOfExperience,
    decimal ServiceRadiusKm,
    decimal? HourlyRateMin,
    decimal? HourlyRateMax,
    string? InsuranceProvider,
    string? InsurancePolicyNumber,
    DateTime? InsuranceExpiryDate,
    bool HasPoliceClearance,
    DateTime? PoliceClearanceDate,
    string? BusinessRegistrationNumber,
    string? VatNumber) : IRequest<bool>;

public record ToggleAvailabilityCommand(Guid ProfileId, AvailabilityStatus Status) : IRequest<bool>;

public record AddTradespersonSkillCommand(Guid ProfileId, int SkillId, ProficiencyLevel ProficiencyLevel) : IRequest<Guid>;
public record RemoveTradespersonSkillCommand(Guid ProfileId, Guid SkillEntryId) : IRequest<bool>;

public record AddExperienceCommand(Guid ProfileId, string Title, string? CompanyName, string? Description, DateTime StartDate, DateTime? EndDate, bool IsCurrent) : IRequest<Guid>;
public record UpdateExperienceCommand(Guid ExperienceId, string Title, string? CompanyName, string? Description, DateTime StartDate, DateTime? EndDate, bool IsCurrent) : IRequest<bool>;
public record DeleteExperienceCommand(Guid ExperienceId) : IRequest<bool>;

public record AddLicenseCommand(Guid ProfileId, string LicenseName, string LicenseNumber, string? IssuingAuthority, DateTime? IssueDate, DateTime? ExpiryDate) : IRequest<Guid>;
public record AddCertificateCommand(Guid ProfileId, string Name, string? IssuingOrganization, DateTime? IssueDate, DateTime? ExpiryDate, string? CredentialId) : IRequest<Guid>;
public record AddLanguageCommand(Guid ProfileId, string LanguageCode, string LanguageName, LanguageProficiency Proficiency) : IRequest<Guid>;
public record AddServiceAreaCommand(Guid ProfileId, string AreaName, decimal CenterLatitude, decimal CenterLongitude, decimal RadiusKm) : IRequest<Guid>;
public record SetAvailabilityScheduleCommand(Guid ProfileId, IReadOnlyList<AvailabilitySlotDto> Slots) : IRequest<bool>;

public record GetTradespersonProfileQuery(Guid ProfileId) : IRequest<TradespersonProfileDto?>;
public record GetMyTradespersonProfileQuery() : IRequest<TradespersonProfileDto?>;

public class AvailabilitySlotDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; }
}

public class TradespersonProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? BusinessName { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public int YearsOfExperience { get; set; }
    public string VerificationStatus { get; set; } = string.Empty;
    public string AvailabilityStatus { get; set; } = string.Empty;
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int CompletedJobsCount { get; set; }
    public decimal ServiceRadiusKm { get; set; }
    public decimal? HourlyRateMin { get; set; }
    public decimal? HourlyRateMax { get; set; }
    public string? InsuranceProvider { get; set; }
    public bool HasPoliceClearance { get; set; }
    public string? BusinessRegistrationNumber { get; set; }
    public int ProfileCompleteness { get; set; }
    public IReadOnlyList<TradespersonSkillDto> Skills { get; set; } = Array.Empty<TradespersonSkillDto>();
    public IReadOnlyList<ExperienceDto> Experiences { get; set; } = Array.Empty<ExperienceDto>();
    public IReadOnlyList<LicenseDto> Licenses { get; set; } = Array.Empty<LicenseDto>();
    public IReadOnlyList<CertificateDto> Certificates { get; set; } = Array.Empty<CertificateDto>();
    public IReadOnlyList<LanguageDto> Languages { get; set; } = Array.Empty<LanguageDto>();
}

public class TradespersonSkillDto { public Guid Id { get; set; } public int SkillId { get; set; } public string SkillName { get; set; } = string.Empty; public string ProficiencyLevel { get; set; } = string.Empty; }
public class ExperienceDto { public Guid Id { get; set; } public string Title { get; set; } = string.Empty; public string? CompanyName { get; set; } public string? Description { get; set; } public DateTime StartDate { get; set; } public DateTime? EndDate { get; set; } public bool IsCurrent { get; set; } }
public class LicenseDto { public Guid Id { get; set; } public string LicenseName { get; set; } = string.Empty; public string LicenseNumber { get; set; } = string.Empty; public bool IsVerified { get; set; } }
public class CertificateDto { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; public string? IssuingOrganization { get; set; } public bool IsVerified { get; set; } }
public class LanguageDto { public Guid Id { get; set; } public string LanguageCode { get; set; } = string.Empty; public string LanguageName { get; set; } = string.Empty; public string Proficiency { get; set; } = string.Empty; }

public class UpdateTradespersonProfileCommandValidator : AbstractValidator<UpdateTradespersonProfileCommand>
{
    public UpdateTradespersonProfileCommandValidator()
    {
        RuleFor(x => x.ProfileId).NotEmpty();
        RuleFor(x => x.ServiceRadiusKm).GreaterThan(0).LessThanOrEqualTo(200);
    }
}
