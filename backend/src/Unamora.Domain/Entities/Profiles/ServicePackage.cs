using Unamora.Domain.Common;
using Unamora.Domain.Entities.Catalog;

namespace Unamora.Domain.Entities.Profiles;

public class ServicePackage : BaseEntity
{
    public Guid TradespersonProfileId { get; set; }
    public TradespersonProfile TradespersonProfile { get; set; } = null!;
    public int ServiceCategoryId { get; set; }
    public ServiceCategory ServiceCategory { get; set; } = null!;
    public string TierName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "ZAR";
    public int EstimatedDurationMinutes { get; set; }
    public string? InclusionsJson { get; set; }
    public string? ExclusionsJson { get; set; }
    public bool IsInstantBook { get; set; }
    public bool IsActive { get; set; } = true;
}
