namespace Unamora.Application.Modules.Payments.DTOs;

public class CreatePaymentDto
{
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public int PaymentMethod { get; set; }
    public bool UseEscrow { get; set; } = false;
    public string? CardToken { get; set; }
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid PayerId { get; set; }
    public Guid PayeeId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public int Status { get; set; }
    public int PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public DateTime ProcessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CreateInvoiceDto
{
    public Guid PaymentId { get; set; }
    public Guid RecipientId { get; set; }
    public string Description { get; set; }
    public List<InvoiceLineItemDto> LineItems { get; set; }
    public DateTime DueDate { get; set; }
    public string? Notes { get; set; }
}

public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public string InvoiceNumber { get; set; }
    public Guid IssuerId { get; set; }
    public Guid RecipientId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public int Status { get; set; }
    public string Description { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public List<InvoiceLineItemDto> LineItems { get; set; }
    public string? Notes { get; set; }
}

public class InvoiceLineItemDto
{
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class EscrowAccountDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid HolderUserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public DateTime FundsLockedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public bool IsReleased { get; set; }
}

public class ReleaseEscrowDto
{
    public Guid EscrowAccountId { get; set; }
    public string Reason { get; set; }
}

public class CreateRefundDto
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
}

public class RefundDto
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public int Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class PaymentReceiptDto
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public string ReceiptNumber { get; set; }
    public string ContentUrl { get; set; }
    public DateTime IssuedAt { get; set; }
}

public class CreateSubscriptionDto
{
    public int Tier { get; set; }
    public bool BillingAnnually { get; set; } = false;
    public int? CardToken { get; set; }
}

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Tier { get; set; }
    public int Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? RenewalDate { get; set; }
    public decimal MonthlyRate { get; set; }
    public decimal? AnnualRate { get; set; }
    public bool AutoRenew { get; set; }
    public string TierName { get; set; }
    public List<string>? Features { get; set; }
}

public class UpdateSubscriptionDto
{
    public Guid SubscriptionId { get; set; }
    public int? NewTier { get; set; }
    public bool? AutoRenew { get; set; }
}

public class CommissionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BookingId { get; set; }
    public decimal BookingAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public int Status { get; set; }
    public DateTime CalculatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class CommissionStatisticsDto
{
    public Guid UserId { get; set; }
    public decimal TotalEarned { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal PendingAmount { get; set; }
    public int CommissionCount { get; set; }
    public decimal AverageCommissionRate { get; set; }
}

public class PaymentHistoryDto
{
    public Guid PaymentId { get; set; }
    public Guid BookingId { get; set; }
    public decimal Amount { get; set; }
    public int Status { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string BookingDescription { get; set; }
}
