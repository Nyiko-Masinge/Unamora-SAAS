using Unamora.Domain.Common;

namespace Unamora.Domain.Entities.Profiles;

public class TradespersonLicense : BaseEntity
{
    public Guid TradespersonProfileId { get; set; }
    public TradespersonProfile TradespersonProfile { get; set; } = null!;
    public string LicenseName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? IssuingAuthority { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? DocumentUrl { get; set; }
    public bool IsVerified { get; set; }
}
