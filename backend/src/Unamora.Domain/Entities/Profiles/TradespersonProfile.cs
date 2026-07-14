using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Profiles;

public class TradespersonProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string? BusinessName { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public int YearsOfExperience { get; set; }
    public TradespersonVerificationStatus VerificationStatus { get; set; } = TradespersonVerificationStatus.NotSubmitted;
    public DateTime? VerifiedAt { get; set; }
    public AvailabilityStatus AvailabilityStatus { get; set; } = AvailabilityStatus.Offline;
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int CompletedJobsCount { get; set; }
    public int? ResponseTimeMinutes { get; set; }
    public decimal? BaseLatitude { get; set; }
    public decimal? BaseLongitude { get; set; }
    public decimal ServiceRadiusKm { get; set; } = 25;
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Free;
    public decimal? HourlyRateMin { get; set; }
    public decimal? HourlyRateMax { get; set; }
    public string? InsuranceProvider { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public DateTime? InsuranceExpiryDate { get; set; }
    public bool HasPoliceClearance { get; set; }
    public DateTime? PoliceClearanceDate { get; set; }
    public string? BusinessRegistrationNumber { get; set; }
    public string? VatNumber { get; set; }
    public int ProfileCompleteness { get; set; }

    public ICollection<TradespersonSkill> Skills { get; set; } = new List<TradespersonSkill>();
    public ICollection<TradespersonExperience> Experiences { get; set; } = new List<TradespersonExperience>();
    public ICollection<TradespersonLicense> Licenses { get; set; } = new List<TradespersonLicense>();
    public ICollection<TradespersonCertificate> Certificates { get; set; } = new List<TradespersonCertificate>();
    public ICollection<TradespersonLanguage> Languages { get; set; } = new List<TradespersonLanguage>();
    public ICollection<TradespersonServiceArea> ServiceAreas { get; set; } = new List<TradespersonServiceArea>();
    public ICollection<TradespersonAvailability> Availabilities { get; set; } = new List<TradespersonAvailability>();
    public ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();
    public ICollection<PortfolioProject> PortfolioProjects { get; set; } = new List<PortfolioProject>();
}
