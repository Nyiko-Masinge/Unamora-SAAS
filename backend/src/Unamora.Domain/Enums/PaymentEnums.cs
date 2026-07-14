namespace Unamora.Domain.Enums;

public enum PaymentStatus
{
    Pending = 0,
    Processed = 1,
    Failed = 2,
    Refunded = 3,
    Disputed = 4,
    Completed = 5
}

public enum PaymentMethodType
{
    CreditCard = 0,
    DebitCard = 1,
    BankTransfer = 2,
    MobileWallet = 3,
    PayPal = 4,
    ApplePay = 5,
    GooglePay = 6
}

public enum InvoiceStatus
{
    Draft = 0,
    Sent = 1,
    Viewed = 2,
    PartiallyPaid = 3,
    Paid = 4,
    Overdue = 5,
    Cancelled = 6
}

public enum RefundStatus
{
    Requested = 0,
    Approved = 1,
    Processing = 2,
    Completed = 3,
    Rejected = 4,
    Failed = 5
}

public enum SubscriptionStatus
{
    Active = 0,
    Paused = 1,
    Cancelled = 2,
    Expired = 3,
    PendingActivation = 4
}

public enum SubscriptionTierType
{
    Free = 0,
    Professional = 1,
    Premium = 2,
    Enterprise = 3
}

public enum CommissionStatus
{
    Pending = 0,
    Calculated = 1,
    Paid = 2,
    Withheld = 3
}
