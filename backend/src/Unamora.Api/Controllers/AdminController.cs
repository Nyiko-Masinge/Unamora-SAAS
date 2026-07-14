using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Modules.Admin.DTOs;
using Unamora.Application.Modules.Admin.Services;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminController : ControllerBase
{
    private readonly IAdminDashboardService _dashboardService;
    private readonly IVerificationQueueService _verificationService;
    private readonly IDisputeService _disputeService;
    private readonly IAdminUserService _adminUserService;
    private readonly IUserManagementService _userManagementService;
    private readonly IBookingManagementService _bookingManagementService;
    private readonly IPaymentManagementService _paymentManagementService;
    private readonly IReportsService _reportsService;
    private readonly ICurrentUserService _currentUserService;

    public AdminController(
        IAdminDashboardService dashboardService,
        IVerificationQueueService verificationService,
        IDisputeService disputeService,
        IAdminUserService adminUserService,
        IUserManagementService userManagementService,
        IBookingManagementService bookingManagementService,
        IPaymentManagementService paymentManagementService,
        IReportsService reportsService,
        ICurrentUserService currentUserService)
    {
        _dashboardService = dashboardService;
        _verificationService = verificationService;
        _disputeService = disputeService;
        _adminUserService = adminUserService;
        _userManagementService = userManagementService;
        _bookingManagementService = bookingManagementService;
        _paymentManagementService = paymentManagementService;
        _reportsService = reportsService;
        _currentUserService = currentUserService;
    }

    // Dashboard endpoints
    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();
        return Ok(dashboard);
    }

    [HttpGet("dashboard/statistics")]
    public async Task<ActionResult<DashboardStatisticsDto>> GetStatistics()
    {
        var statistics = await _dashboardService.GetStatisticsAsync();
        return Ok(statistics);
    }

    [HttpGet("dashboard/revenue")]
    public async Task<ActionResult<RevenueChartDataDto>> GetRevenueChart([FromQuery] string period = "monthly")
    {
        var data = await _dashboardService.GetRevenueChartAsync(period);
        return Ok(data);
    }

    [HttpGet("dashboard/user-growth")]
    public async Task<ActionResult<UserGrowthDataDto>> GetUserGrowth()
    {
        var data = await _dashboardService.GetUserGrowthDataAsync();
        return Ok(data);
    }

    [HttpGet("dashboard/recent-activities")]
    public async Task<ActionResult<List<RecentActivityDto>>> GetRecentActivities([FromQuery] int count = 20)
    {
        var activities = await _dashboardService.GetRecentActivitiesAsync(count);
        return Ok(activities);
    }

    // Verification Queue endpoints
    [HttpGet("verifications")]
    public async Task<ActionResult<PendingVerificationsDto>> GetPendingVerifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var verifications = await _verificationService.GetPendingVerificationsAsync(pageNumber, pageSize);
        return Ok(verifications);
    }

    [HttpGet("verification/{id}")]
    public async Task<ActionResult<VerificationQueueDto>> GetVerification(Guid id)
    {
        var verification = await _verificationService.GetVerificationAsync(id);
        return Ok(verification);
    }

    [HttpPost("verification/{id}/approve")]
    public async Task<IActionResult> ApproveVerification(Guid id, [FromBody] ApproveVerificationDto dto)
    {
        var adminUserId = _currentUserService.UserId;
        dto.VerificationQueueId = id;
        await _verificationService.ApproveVerificationAsync(dto, adminUserId);
        return NoContent();
    }

    [HttpPost("verification/{id}/reject")]
    public async Task<IActionResult> RejectVerification(Guid id, [FromBody] RejectVerificationDto dto)
    {
        var adminUserId = _currentUserService.UserId;
        dto.VerificationQueueId = id;
        await _verificationService.RejectVerificationAsync(dto, adminUserId);
        return NoContent();
    }

    [HttpGet("verification/user/{userId}")]
    public async Task<ActionResult<List<VerificationQueueDto>>> GetUserVerificationHistory(Guid userId)
    {
        var history = await _verificationService.GetUserVerificationHistoryAsync(userId);
        return Ok(history);
    }

    // Dispute endpoints
    [HttpPost("disputes")]
    [AllowAnonymous]
    public async Task<ActionResult<DisputeDto>> CreateDispute([FromBody] CreateDisputeDto dto)
    {
        var userId = _currentUserService.UserId;
        var dispute = await _disputeService.CreateDisputeAsync(dto, userId);
        return CreatedAtAction(nameof(GetDispute), new { id = dispute.Id }, dispute);
    }

    [HttpGet("dispute/{id}")]
    public async Task<ActionResult<DisputeDto>> GetDispute(Guid id)
    {
        var dispute = await _disputeService.GetDisputeAsync(id);
        return Ok(dispute);
    }

    [HttpPost("disputes/filter")]
    public async Task<ActionResult<List<DisputeDto>>> FilterDisputes([FromBody] DisputeFiltersDto filters)
    {
        var disputes = await _disputeService.GetDisputesAsync(filters);
        return Ok(disputes);
    }

    [HttpPost("dispute/{id}/assign")]
    public async Task<ActionResult<DisputeDto>> AssignDispute(Guid id)
    {
        var adminUserId = _currentUserService.UserId;
        var dispute = await _disputeService.AssignDisputeAsync(id, adminUserId);
        return Ok(dispute);
    }

    [HttpPost("dispute/{id}/evidence")]
    public async Task<IActionResult> AddEvidence(Guid id, [FromBody] DisputeEvidenceInputDto evidence)
    {
        var userId = _currentUserService.UserId;
        await _disputeService.AddEvidenceAsync(id, evidence, userId);
        return NoContent();
    }

    [HttpPost("dispute/comment")]
    public async Task<IActionResult> AddDisputeComment([FromBody] AddDisputeCommentDto dto)
    {
        var userId = _currentUserService.UserId;
        await _disputeService.AddCommentAsync(dto, userId);
        return NoContent();
    }

    [HttpPost("dispute/resolve")]
    public async Task<IActionResult> ResolveDispute([FromBody] ResolveDisputeDto dto)
    {
        var adminUserId = _currentUserService.UserId;
        await _disputeService.ResolveDisputeAsync(dto, adminUserId);
        return NoContent();
    }

    [HttpPost("dispute/appeal")]
    public async Task<IActionResult> CreateAppeal([FromBody] CreateDisputeAppealDto dto)
    {
        var userId = _currentUserService.UserId;
        await _disputeService.CreateAppealAsync(dto, userId);
        return NoContent();
    }

    [HttpPost("dispute/appeal/{appealId}/review")]
    public async Task<IActionResult> ReviewAppeal(Guid appealId, [FromBody] dynamic reviewData)
    {
        int status = reviewData.status;
        string? decision = reviewData.decision;
        var adminUserId = _currentUserService.UserId;
        await _disputeService.ReviewAppealAsync(appealId, status, decision, adminUserId);
        return NoContent();
    }

    [HttpGet("user/{userId}/disputes")]
    public async Task<ActionResult<List<DisputeDto>>> GetUserDisputes(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var disputes = await _disputeService.GetUserDisputesAsync(userId, pageNumber, pageSize);
        return Ok(disputes);
    }

    // Admin User Management
    [HttpPost("users")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<AdminUserDto>> CreateAdminUser([FromBody] dynamic userData)
    {
        Guid userId = userData.userId;
        int role = userData.role;
        string department = userData.department;
        var adminUser = await _adminUserService.CreateAdminUserAsync(userId, role, department);
        return CreatedAtAction(nameof(GetAdminUser), new { id = adminUser.Id }, adminUser);
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<AdminUserDto>> GetAdminUser(Guid id)
    {
        var adminUser = await _adminUserService.GetAdminUserAsync(id);
        return Ok(adminUser);
    }

    [HttpGet("users")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<List<AdminUserDto>>> GetAllAdminUsers()
    {
        var adminUsers = await _adminUserService.GetAllAdminUsersAsync();
        return Ok(adminUsers);
    }

    [HttpPut("users/{id}/role")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UpdateAdminRole(Guid id, [FromBody] int newRole)
    {
        await _adminUserService.UpdateAdminRoleAsync(id, newRole);
        return NoContent();
    }

    [HttpDelete("users/{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> DeactivateAdminUser(Guid id)
    {
        await _adminUserService.DeactivateAdminUserAsync(id);
        return NoContent();
    }

    [HttpGet("activity-log")]
    public async Task<ActionResult<List<AdminActionLogDto>>> GetActionLog([FromQuery] Guid? adminUserId = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var log = await _adminUserService.GetAdminActionLogAsync(adminUserId, pageNumber, pageSize);
        return Ok(log);
    }

    // User Management
    [HttpGet("platform-users")]
    public async Task<ActionResult<List<UserManagementDto>>> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var users = await _userManagementService.GetAllUsersAsync(pageNumber, pageSize);
        return Ok(users);
    }

    [HttpGet("platform-users/{id}")]
    public async Task<ActionResult<UserManagementDto>> GetPlatformUser(Guid id)
    {
        var user = await _userManagementService.GetUserAsync(id);
        return Ok(user);
    }

    [HttpPost("platform-users/{id}/suspend")]
    public async Task<IActionResult> SuspendUser(Guid id, [FromBody] string reason)
    {
        var adminUserId = _currentUserService.UserId;
        await _userManagementService.SuspendUserAsync(id, reason, adminUserId);
        return NoContent();
    }

    [HttpPost("platform-users/{id}/activate")]
    public async Task<IActionResult> ActivateUser(Guid id)
    {
        await _userManagementService.ActivateUserAsync(id);
        return NoContent();
    }

    [HttpDelete("platform-users/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var adminUserId = _currentUserService.UserId;
        await _userManagementService.DeleteUserAsync(id, adminUserId);
        return NoContent();
    }

    [HttpPost("platform-users/search")]
    public async Task<ActionResult<List<UserManagementDto>>> SearchUsers([FromBody] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var users = await _userManagementService.SearchUsersAsync(searchTerm, pageNumber, pageSize);
        return Ok(users);
    }

    // Booking Management
    [HttpGet("bookings")]
    public async Task<ActionResult<List<BookingManagementDto>>> GetAllBookings([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var bookings = await _bookingManagementService.GetAllBookingsAsync(pageNumber, pageSize);
        return Ok(bookings);
    }

    [HttpGet("booking/{id}")]
    public async Task<ActionResult<BookingManagementDto>> GetBooking(Guid id)
    {
        var booking = await _bookingManagementService.GetBookingAsync(id);
        return Ok(booking);
    }

    [HttpGet("bookings/status/{status}")]
    public async Task<ActionResult<List<BookingManagementDto>>> GetBookingsByStatus(int status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var bookings = await _bookingManagementService.GetBookingsByStatusAsync(status, pageNumber, pageSize);
        return Ok(bookings);
    }

    [HttpGet("bookings/date-range")]
    public async Task<ActionResult<List<BookingManagementDto>>> GetBookingsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var bookings = await _bookingManagementService.GetBookingsByDateRangeAsync(startDate, endDate);
        return Ok(bookings);
    }

    // Payment Management
    [HttpGet("payments")]
    public async Task<ActionResult<List<PaymentManagementDto>>> GetAllPayments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var payments = await _paymentManagementService.GetAllPaymentsAsync(pageNumber, pageSize);
        return Ok(payments);
    }

    [HttpGet("payment/{id}")]
    public async Task<ActionResult<PaymentManagementDto>> GetPayment(Guid id)
    {
        var payment = await _paymentManagementService.GetPaymentAsync(id);
        return Ok(payment);
    }

    [HttpGet("payments/status/{status}")]
    public async Task<ActionResult<List<PaymentManagementDto>>> GetPaymentsByStatus(int status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var payments = await _paymentManagementService.GetPaymentsByStatusAsync(status, pageNumber, pageSize);
        return Ok(payments);
    }

    [HttpPost("payment/{id}/approve")]
    public async Task<IActionResult> ApprovePayment(Guid id)
    {
        var adminUserId = _currentUserService.UserId;
        await _paymentManagementService.ApprovePaymentAsync(id, adminUserId);
        return NoContent();
    }

    [HttpPost("payment/{id}/reject")]
    public async Task<IActionResult> RejectPayment(Guid id, [FromBody] string reason)
    {
        var adminUserId = _currentUserService.UserId;
        await _paymentManagementService.RejectPaymentAsync(id, reason, adminUserId);
        return NoContent();
    }

    // Reports
    [HttpGet("reports/revenue")]
    public async Task<ActionResult<byte[]>> GenerateRevenueReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string format = "pdf")
    {
        var report = await _reportsService.GenerateRevenueReportAsync(startDate, endDate, format);
        return File(report, "application/octet-stream", "revenue-report.pdf");
    }

    [HttpGet("reports/users")]
    public async Task<ActionResult<byte[]>> GenerateUserReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string format = "pdf")
    {
        var report = await _reportsService.GenerateUserReportAsync(startDate, endDate, format);
        return File(report, "application/octet-stream", "user-report.pdf");
    }

    [HttpGet("reports/bookings")]
    public async Task<ActionResult<byte[]>> GenerateBookingReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string format = "pdf")
    {
        var report = await _reportsService.GenerateBookingReportAsync(startDate, endDate, format);
        return File(report, "application/octet-stream", "booking-report.pdf");
    }

    [HttpGet("reports/payments")]
    public async Task<ActionResult<byte[]>> GeneratePaymentReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string format = "pdf")
    {
        var report = await _reportsService.GeneratePaymentReportAsync(startDate, endDate, format);
        return File(report, "application/octet-stream", "payment-report.pdf");
    }

    [HttpGet("reports/disputes")]
    public async Task<ActionResult<byte[]>> GenerateDisputeReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string format = "pdf")
    {
        var report = await _reportsService.GenerateDisputeReportAsync(startDate, endDate, format);
        return File(report, "application/octet-stream", "dispute-report.pdf");
    }

    [HttpGet("reports/comprehensive")]
    public async Task<ActionResult<byte[]>> GenerateComprehensiveReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string format = "pdf")
    {
        var report = await _reportsService.GenerateComprehensiveReportAsync(startDate, endDate, format);
        return File(report, "application/octet-stream", "comprehensive-report.pdf");
    }
}
