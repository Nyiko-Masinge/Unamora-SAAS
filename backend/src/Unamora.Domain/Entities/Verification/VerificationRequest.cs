using Unamora.Domain.Common;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Verification;

public class VerificationRequest : BaseEntity
{
    public Guid TradespersonProfileId { get; set; }
    public TradespersonProfile TradespersonProfile { get; set; } = null!;
    public TradespersonVerificationStatus Status { get; set; } = TradespersonVerificationStatus.Submitted;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Notes { get; set; }
    public ICollection<VerificationDocument> Documents { get; set; } = new List<VerificationDocument>();
    public ICollection<VerificationReviewLog> ReviewLogs { get; set; } = new List<VerificationReviewLog>();
}
