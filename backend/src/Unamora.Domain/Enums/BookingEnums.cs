namespace Unamora.Domain.Enums;

public enum JobRequestStatus
{
    Pending = 0,
    Matched = 1,
    Assigned = 2,
    Cancelled = 3
}

public enum QuoteStatus
{
    Pending = 0,
    Sent = 1,
    Accepted = 2,
    Declined = 3,
    Withdrawn = 4
}

public enum BookingStatus
{
    Pending = 0,
    Accepted = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4
}

public enum TrackingStatus
{
    EnRoute = 0,
    Arrived = 1,
    WorkStarted = 2,
    WorkCompleted = 3
}

public enum ReviewDirection
{
    ClientToTradesperson = 0,
    TradespersonToClient = 1
}
