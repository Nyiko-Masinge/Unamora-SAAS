using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Modules.Payments.DTOs;
using Unamora.Application.Modules.Payments.Services;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IInvoiceService _invoiceService;
    private readonly IEscrowService _escrowService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ICommissionService _commissionService;
    private readonly ICurrentUserService _currentUserService;

    public PaymentController(
        IPaymentService paymentService,
        IInvoiceService invoiceService,
        IEscrowService escrowService,
        ISubscriptionService subscriptionService,
        ICommissionService commissionService,
        ICurrentUserService currentUserService)
    {
        _paymentService = paymentService;
        _invoiceService = invoiceService;
        _escrowService = escrowService;
        _subscriptionService = subscriptionService;
        _commissionService = commissionService;
        _currentUserService = currentUserService;
    }

    // Payment endpoints
    [HttpPost("process")]
    public async Task<ActionResult<PaymentDto>> ProcessPayment([FromBody] CreatePaymentDto dto)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var payment = await _paymentService.CreatePaymentAsync(dto, userId);
        await _paymentService.ProcessPaymentAsync(payment.Id);
        return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(Guid id)
    {
        var payment = await _paymentService.GetPaymentAsync(id);
        return Ok(payment);
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<PaymentHistoryDto>>> GetPaymentHistory([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var history = await _paymentService.GetPaymentHistoryAsync(userId, pageNumber, pageSize);
        return Ok(history);
    }

    [HttpPost("{id}/receipt")]
    public async Task<ActionResult<PaymentReceiptDto>> GenerateReceipt(Guid id)
    {
        var receipt = await _paymentService.GenerateReceiptAsync(id);
        return Ok(receipt);
    }

    [HttpPost("{id}/verify")]
    public async Task<ActionResult<bool>> VerifyPayment(Guid id, [FromBody] string transactionId)
    {
        var result = await _paymentService.VerifyPaymentAsync(id, transactionId);
        return Ok(result);
    }

    // Refund endpoints
    [HttpPost("refund")]
    public async Task<ActionResult<RefundDto>> CreateRefund([FromBody] CreateRefundDto dto)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var refund = await _paymentService.CreateRefundAsync(dto, userId);
        return CreatedAtAction(nameof(GetRefund), new { id = refund.Id }, refund);
    }

    [HttpGet("refund/{id}")]
    public async Task<ActionResult<RefundDto>> GetRefund(Guid id)
    {
        var refund = await _paymentService.GetRefundAsync(id);
        return Ok(refund);
    }

    [HttpPost("refund/{id}/process")]
    public async Task<IActionResult> ProcessRefund(Guid id)
    {
        await _paymentService.ProcessRefundAsync(id);
        return NoContent();
    }

    // Invoice endpoints
    [HttpPost("invoice")]
    public async Task<ActionResult<InvoiceDto>> CreateInvoice([FromBody] CreateInvoiceDto dto)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var invoice = await _invoiceService.CreateInvoiceAsync(dto, userId);
        return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
    }

    [HttpGet("invoice/{id}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(Guid id)
    {
        var invoice = await _invoiceService.GetInvoiceAsync(id);
        return Ok(invoice);
    }

    [HttpGet("invoices")]
    public async Task<ActionResult<List<InvoiceDto>>> GetInvoices([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var invoices = await _invoiceService.GetUserInvoicesAsync(userId, pageNumber, pageSize);
        return Ok(invoices);
    }

    [HttpPut("invoice/{id}")]
    public async Task<ActionResult<InvoiceDto>> UpdateInvoice(Guid id, [FromBody] CreateInvoiceDto dto)
    {
        var invoice = await _invoiceService.UpdateInvoiceAsync(id, dto);
        return Ok(invoice);
    }

    [HttpPost("invoice/{id}/send")]
    public async Task<IActionResult> SendInvoice(Guid id)
    {
        await _invoiceService.SendInvoiceAsync(id);
        return NoContent();
    }

    [HttpPost("invoice/{id}/paid")]
    public async Task<IActionResult> MarkInvoiceAsPaid(Guid id)
    {
        await _invoiceService.MarkInvoiceAsPaidAsync(id);
        return NoContent();
    }

    [HttpDelete("invoice/{id}")]
    public async Task<IActionResult> DeleteInvoice(Guid id)
    {
        await _invoiceService.DeleteInvoiceAsync(id);
        return NoContent();
    }

    [HttpGet("invoice/{id}/pdf")]
    public async Task<ActionResult<byte[]>> GenerateInvoicePdf(Guid id)
    {
        var pdf = await _invoiceService.GenerateInvoicePdfAsync(id);
        return File(pdf, "application/pdf", "invoice.pdf");
    }

    // Escrow endpoints
    [HttpPost("escrow")]
    public async Task<ActionResult<EscrowAccountDto>> CreateEscrow([FromBody] CreatePaymentDto dto)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var payment = await _paymentService.CreatePaymentAsync(dto, userId);
        var escrow = await _escrowService.CreateEscrowAsync(payment.BookingId, payment.Amount, userId);
        return CreatedAtAction(nameof(GetEscrow), new { id = escrow.Id }, escrow);
    }

    [HttpGet("escrow/{id}")]
    public async Task<ActionResult<EscrowAccountDto>> GetEscrow(Guid id)
    {
        var escrow = await _escrowService.GetEscrowAsync(id);
        return Ok(escrow);
    }

    [HttpPost("escrow/release")]
    public async Task<IActionResult> ReleaseEscrow([FromBody] ReleaseEscrowDto dto)
    {
        var adminUserId = _currentUserService.UserId ?? Guid.Empty;
        await _escrowService.ReleaseEscrowAsync(dto, adminUserId);
        return NoContent();
    }

    [HttpGet("booking/{bookingId}/escrow")]
    public async Task<ActionResult<EscrowAccountDto>> GetBookingEscrow(Guid bookingId)
    {
        var escrow = await _escrowService.GetBookingEscrowAsync(bookingId);
        return Ok(escrow);
    }

    // Subscription endpoints
    [HttpPost("subscription")]
    public async Task<ActionResult<SubscriptionDto>> CreateSubscription([FromBody] CreateSubscriptionDto dto)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var subscription = await _subscriptionService.CreateSubscriptionAsync(dto, userId);
        return CreatedAtAction(nameof(GetSubscription), new { id = subscription.Id }, subscription);
    }

    [HttpGet("subscription")]
    public async Task<ActionResult<SubscriptionDto>> GetSubscription()
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var subscription = await _subscriptionService.GetUserSubscriptionAsync(userId);
        return Ok(subscription);
    }

    [HttpPut("subscription")]
    public async Task<ActionResult<SubscriptionDto>> UpdateSubscription([FromBody] UpdateSubscriptionDto dto)
    {
        var subscription = await _subscriptionService.UpdateSubscriptionAsync(dto);
        return Ok(subscription);
    }

    [HttpDelete("subscription/{id}")]
    public async Task<IActionResult> CancelSubscription(Guid id)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        await _subscriptionService.CancelSubscriptionAsync(id, userId);
        return NoContent();
    }

    [HttpPost("subscription/{id}/upgrade")]
    public async Task<ActionResult<SubscriptionDto>> UpgradeSubscription(Guid id, [FromBody] int newTier)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var subscription = await _subscriptionService.UpgradeSubscriptionAsync(id, newTier, userId);
        return Ok(subscription);
    }

    [HttpGet("subscriptions/available")]
    [AllowAnonymous]
    public async Task<ActionResult<List<SubscriptionDto>>> GetAvailableSubscriptions()
    {
        var subscriptions = await _subscriptionService.GetAvailableSubscriptionsAsync();
        return Ok(subscriptions);
    }

    // Commission endpoints
    [HttpGet("commissions")]
    public async Task<ActionResult<List<CommissionDto>>> GetCommissions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var commissions = await _commissionService.GetUserCommissionsAsync(userId, pageNumber, pageSize);
        return Ok(commissions);
    }

    [HttpGet("commission/{id}")]
    public async Task<ActionResult<CommissionDto>> GetCommission(Guid id)
    {
        var commission = await _commissionService.GetCommissionAsync(id);
        return Ok(commission);
    }

    [HttpGet("commissions/statistics")]
    public async Task<ActionResult<CommissionStatisticsDto>> GetCommissionStatistics()
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var statistics = await _commissionService.GetCommissionStatisticsAsync(userId);
        return Ok(statistics);
    }

    [HttpPost("commission/{id}/pay")]
    public async Task<IActionResult> PayCommission(Guid id)
    {
        var adminUserId = _currentUserService.UserId ?? Guid.Empty;
        await _commissionService.PayCommissionAsync(id, adminUserId);
        return NoContent();
    }

    [HttpPost("commission/{id}/withhold")]
    public async Task<IActionResult> WithholdCommission(Guid id, [FromBody] string reason)
    {
        await _commissionService.WithholdCommissionAsync(id, reason);
        return NoContent();
    }

    [HttpPost("commission/{id}/release")]
    public async Task<IActionResult> ReleaseWithheldCommission(Guid id)
    {
        await _commissionService.ReleaseWithheldCommissionAsync(id);
        return NoContent();
    }

    [HttpPost("commission/{id}/adjust")]
    public async Task<IActionResult> AdjustCommission(Guid id, [FromBody] dynamic adjustmentData)
    {
        decimal amount = adjustmentData.amount;
        string reason = adjustmentData.reason;
        await _commissionService.AdjustCommissionAsync(id, amount, reason);
        return NoContent();
    }
}
