using Unamora.Domain.Common;

namespace Unamora.Domain.Entities.Profiles;

public class TradespersonCertificate : BaseEntity
{
    public Guid TradespersonProfileId { get; set; }
    public TradespersonProfile TradespersonProfile { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? IssuingOrganization { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CredentialId { get; set; }
    public string? DocumentUrl { get; set; }
    public bool IsVerified { get; set; }
}
