namespace Unamora.Domain.Enums;

public enum ReviewStatus
{
    Pending = 0,
    Published = 1,
    Flagged = 2,
    Hidden = 3,
    Deleted = 4
}

public enum RatingCategory
{
    Quality = 0,
    Communication = 1,
    Timeliness = 2,
    Professionalism = 3,
    Value = 4
}

public enum SpamDetectionStatus
{
    Approved = 0,
    UnderReview = 1,
    Flagged = 2,
    AutoRemoved = 3
}
