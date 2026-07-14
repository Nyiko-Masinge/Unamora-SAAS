using Unamora.Domain.Common;

namespace Unamora.Domain.Entities.Verification;

public class VerificationReviewLog : BaseEntity
{
    public Guid VerificationRequestId { get; set; }
    public VerificationRequest VerificationRequest { get; set; } = null!;
    public string Action { get; set; } = string.Empty;
    public Guid PerformedByUserId { get; set; }
    public string? Notes { get; set; }
}
