using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Payments;

public class Subscription : BaseEntity
{
    public Guid UserId { get; set; }
    public SubscriptionTierType Tier { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.PendingActivation;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? RenewalDate { get; set; }
    public decimal MonthlyRate { get; set; }
    public decimal? AnnualRate { get; set; }
    public bool AutoRenew { get; set; } = true;
    public int BillingCycleDays { get; set; } = 30;
    
    public ICollection<SubscriptionPayment> Payments { get; set; } = new List<SubscriptionPayment>();
}

public class SubscriptionPayment : BaseEntity
{
    public Guid SubscriptionId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? TransactionId { get; set; }
    
    public Subscription Subscription { get; set; }
}

public class Commission : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid BookingId { get; set; }
    public decimal BookingAmount { get; set; }
    public decimal CommissionRate { get; set; } // Percentage (0-100)
    public decimal CommissionAmount { get; set; }
    public CommissionStatus Status { get; set; } = CommissionStatus.Pending;
    public DateTime CalculatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaymentTransactionId { get; set; }
    public DateTime? WithheldUntil { get; set; }
    public string? WithholdReason { get; set; }
    
    public ICollection<CommissionAdjustment> Adjustments { get; set; } = new List<CommissionAdjustment>();
}

public class CommissionAdjustment : BaseEntity
{
    public Guid CommissionId { get; set; }
    public decimal AdjustmentAmount { get; set; }
    public string Reason { get; set; }
    public DateTime AdjustedAt { get; set; }
    
    public Commission Commission { get; set; }
}
