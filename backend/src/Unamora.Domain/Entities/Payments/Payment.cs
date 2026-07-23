using Unamora.Domain.Common;
using Unamora.Domain.Entities.Bookings;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Payments;

public class Payment : BaseEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public Guid PayerId { get; set; }
    public Guid PayeeId { get; set; }
    public Guid? EscrowAccountId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public PaymentMethodType PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
    
    public EscrowAccount? EscrowAccount { get; set; }
    public ICollection<PaymentReceipt> Receipts { get; set; } = new List<PaymentReceipt>();
}

public class Invoice : BaseEntity
{
    public Guid PaymentId { get; set; }
    public string InvoiceNumber { get; set; }
    public Guid IssuerId { get; set; }
    public Guid RecipientId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string Description { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }
    
    public Payment Payment { get; set; }
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
}

public class InvoiceLineItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    public int DisplayOrder { get; set; }
    
    public Invoice Invoice { get; set; }
}

public class EscrowAccount : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid HolderUserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime FundsLockedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public bool IsReleased { get; set; } = false;
    public string? ReleaseReason { get; set; }
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class Refund : BaseEntity
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public RefundStatus Status { get; set; } = RefundStatus.Requested;
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? TransactionId { get; set; }
    
    public Payment Payment { get; set; }
}

public class PaymentReceipt : BaseEntity
{
    public Guid PaymentId { get; set; }
    public string ReceiptNumber { get; set; }
    public string ContentUrl { get; set; }
    public DateTime IssuedAt { get; set; }
    
    public Payment Payment { get; set; }
}
