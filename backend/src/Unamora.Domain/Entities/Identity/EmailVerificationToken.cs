using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Identity;

public class EmailVerificationToken : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public VerificationStatus Status { get; set; } = VerificationStatus.EmailSent;
    public DateTime? VerifiedAt { get; set; }
}
