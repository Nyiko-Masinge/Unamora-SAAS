namespace Unamora.Domain.Enums;

public enum AdminRole
{
    SuperAdmin = 0,
    Admin = 1,
    Moderator = 2,
    Analyst = 3
}

public enum DisputeStatus
{
    Opened = 0,
    UnderReview = 1,
    AwaitingEvidence = 2,
    Closed = 3,
    Appealed = 4,
    Resolved = 5
}

public enum DisputeCategory
{
    PaymentIssue = 0,
    ServiceQuality = 1,
    Cancellation = 2,
    NoShow = 3,
    DamageOrLoss = 4,
    Harassment = 5,
    Other = 6
}

public enum DisputeResolution
{
    RefundToClaimant = 0,
    PaymentToRespondent = 1,
    SplitRefund = 2,
    Dismissed = 3,
    Appealed = 4
}

public enum AppealStatus
{
    Pending = 0,
    Reviewed = 1,
    Approved = 2,
    Rejected = 3
}

public enum VerificationQueueStatus
{
    Pending = 0,
    UnderReview = 1,
    Approved = 2,
    Rejected = 3,
    ResubmissionRequired = 4
}
