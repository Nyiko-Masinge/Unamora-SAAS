namespace Unamora.Domain.Enums;

public enum TradespersonVerificationStatus
{
    NotSubmitted = 0,
    Submitted = 1,
    InReview = 2,
    Approved = 3,
    Rejected = 4,
    Suspended = 5,
    Expired = 6
}

public enum SubscriptionTier
{
    Free = 0,
    Pro = 1,
    Business = 2
}

public enum AvailabilityStatus
{
    Offline = 0,
    Online = 1,
    Busy = 2
}

public enum DocumentScanStatus
{
    Pending = 0,
    Processing = 1,
    Clean = 2,
    Rejected = 3,
    Failed = 4
}

public enum VerificationDocumentType
{
    NationalId = 0,
    TradeCertificate = 1,
    Insurance = 2,
    BusinessRegistration = 3,
    PoliceClearance = 4,
    Other = 5
}

public enum PortfolioMediaType
{
    Image = 0,
    Video = 1
}

public enum ProficiencyLevel
{
    Beginner = 0,
    Intermediate = 1,
    Advanced = 2,
    Expert = 3
}

public enum LanguageProficiency
{
    Basic = 0,
    Conversational = 1,
    Fluent = 2,
    Native = 3
}
