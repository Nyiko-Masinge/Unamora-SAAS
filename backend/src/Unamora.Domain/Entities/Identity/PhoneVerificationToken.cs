using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Identity;

public class PhoneVerificationToken : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string OtpHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public VerificationStatus Status { get; set; } = VerificationStatus.EmailSent;
    public int AttemptCount { get; set; }
    public DateTime? VerifiedAt { get; set; }
}
