using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Admin;

public class Dispute : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid ClaimantUserId { get; set; }
    public Guid RespondentUserId { get; set; }
    public Guid? AssignedToAdminId { get; set; }
    public DisputeCategory Category { get; set; }
    public DisputeStatus Status { get; set; } = DisputeStatus.Opened;
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal ClaimedAmount { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public DisputeResolution? Resolution { get; set; }
    public string? ResolutionNotes { get; set; }
    public int AppealCount { get; set; } = 0;
    
    public AdminUser? AssignedToAdmin { get; set; }
    public ICollection<DisputeEvidence> Evidence { get; set; } = new List<DisputeEvidence>();
    public ICollection<DisputeComment> Comments { get; set; } = new List<DisputeComment>();
    public ICollection<DisputeAppeal> Appeals { get; set; } = new List<DisputeAppeal>();
}

public class DisputeEvidence : BaseEntity
{
    public Guid DisputeId { get; set; }
    public Guid SubmittedByUserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; } // image, video, document
    public long FileSizeBytes { get; set; }
    public DateTime SubmittedAt { get; set; }
    
    public Dispute Dispute { get; set; }
}

public class DisputeComment : BaseEntity
{
    public Guid DisputeId { get; set; }
    public Guid AuthorUserId { get; set; }
    public string Content { get; set; }
    public bool IsAdminComment { get; set; } = false;
    public bool IsPublic { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public Dispute Dispute { get; set; }
}

public class DisputeAppeal : BaseEntity
{
    public Guid DisputeId { get; set; }
    public Guid AppealedByUserId { get; set; }
    public string Reason { get; set; }
    public AppealStatus Status { get; set; } = AppealStatus.Pending;
    public DateTime AppealedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? AdminDecision { get; set; }
    
    public Dispute Dispute { get; set; }
}

public class VerificationQueue : BaseEntity
{
    public Guid UserId { get; set; }
    public VerificationQueueStatus Status { get; set; } = VerificationQueueStatus.Pending;
    public string VerificationType { get; set; } // e.g., "IDVerification", "AddressVerification"
    public string? DocumentUrl { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByAdminId { get; set; }
    public string? RejectionReason { get; set; }
    public int AttemptCount { get; set; } = 1;
    
    public AdminUser? ReviewedByAdmin { get; set; }
}
