namespace Unamora.Application.Modules.Admin.DTOs;

// Dashboard DTOs
public class AdminDashboardDto
{
    public DashboardStatisticsDto Statistics { get; set; }
    public List<RecentActivityDto> RecentActivities { get; set; }
    public RevenueChartDataDto RevenueData { get; set; }
    public UserGrowthDataDto UserGrowthData { get; set; }
    public PendingVerificationsDto PendingVerifications { get; set; }
}

public class DashboardStatisticsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalBookings { get; set; }
    public int CompletedBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int OpenDisputes { get; set; }
    public int PendingVerifications { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalPaymentsProcessed { get; set; }
}

public class RecentActivityDto
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public DateTime Timestamp { get; set; }
}

public class RevenueChartDataDto
{
    public List<string> Labels { get; set; }
    public List<decimal> Data { get; set; }
    public string Period { get; set; } // daily, weekly, monthly
}

public class UserGrowthDataDto
{
    public List<string> Labels { get; set; }
    public List<int> ClientsData { get; set; }
    public List<int> TradespeopleData { get; set; }
}

public class PendingVerificationsDto
{
    public int TotalPending { get; set; }
    public List<VerificationQueueDto> Verifications { get; set; }
}

// Verification Queue DTOs
public class VerificationQueueDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string VerificationType { get; set; }
    public int Status { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string? DocumentUrl { get; set; }
    public int AttemptCount { get; set; }
}

public class ApproveVerificationDto
{
    public Guid VerificationQueueId { get; set; }
    public string? Notes { get; set; }
}

public class RejectVerificationDto
{
    public Guid VerificationQueueId { get; set; }
    public string RejectionReason { get; set; }
    public bool RequireResubmission { get; set; } = true;
}

// Dispute DTOs
public class CreateDisputeDto
{
    public Guid BookingId { get; set; }
    public Guid RespondentUserId { get; set; }
    public int Category { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal ClaimedAmount { get; set; }
    public List<DisputeEvidenceInputDto>? Evidence { get; set; }
}

public class DisputeDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid ClaimantUserId { get; set; }
    public string ClaimantName { get; set; }
    public Guid RespondentUserId { get; set; }
    public string RespondentName { get; set; }
    public int Category { get; set; }
    public int Status { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal ClaimedAmount { get; set; }
    public DateTime OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public int? Resolution { get; set; }
    public string? ResolutionNotes { get; set; }
    public int AppealCount { get; set; }
    public Guid? AssignedToAdminId { get; set; }
    public List<DisputeEvidenceDto>? Evidence { get; set; }
    public List<DisputeCommentDto>? Comments { get; set; }
    public List<DisputeAppealDto>? Appeals { get; set; }
}

public class DisputeEvidenceInputDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; } // image, video, document
}

public class DisputeEvidenceDto
{
    public Guid Id { get; set; }
    public Guid DisputeId { get; set; }
    public Guid SubmittedByUserId { get; set; }
    public string SubmittedByName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; }
    public long FileSizeBytes { get; set; }
    public DateTime SubmittedAt { get; set; }
}

public class DisputeCommentDto
{
    public Guid Id { get; set; }
    public Guid DisputeId { get; set; }
    public Guid AuthorUserId { get; set; }
    public string AuthorName { get; set; }
    public string Content { get; set; }
    public bool IsAdminComment { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DisputeAppealDto
{
    public Guid Id { get; set; }
    public Guid DisputeId { get; set; }
    public Guid AppealedByUserId { get; set; }
    public string AppealedByName { get; set; }
    public string Reason { get; set; }
    public int Status { get; set; }
    public DateTime AppealedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? AdminDecision { get; set; }
}

public class AddDisputeCommentDto
{
    public Guid DisputeId { get; set; }
    public string Content { get; set; }
    public bool IsPublic { get; set; } = true;
}

public class ResolveDisputeDto
{
    public Guid DisputeId { get; set; }
    public int Resolution { get; set; }
    public string? ResolutionNotes { get; set; }
}

public class CreateDisputeAppealDto
{
    public Guid DisputeId { get; set; }
    public string Reason { get; set; }
}

public class DisputeFiltersDto
{
    public int? Status { get; set; }
    public int? Category { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? AssignedToAdminId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class AdminUserDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public int Role { get; set; }
    public string? Department { get; set; }
    public bool IsActive { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? RemovedAt { get; set; }
}

public class AdminActionLogDto
{
    public Guid Id { get; set; }
    public Guid AdminUserId { get; set; }
    public string AdminName { get; set; }
    public string ActionType { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? Reason { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? IpAddress { get; set; }
}

public class BookingManagementDto
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string ClientName { get; set; }
    public Guid TradespersonId { get; set; }
    public string TradespersonName { get; set; }
    public string ServiceType { get; set; }
    public int Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime BookedDate { get; set; }
    public DateTime ScheduledDate { get; set; }
    public int TradespersonRating { get; set; }
}

public class UserManagementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string UserType { get; set; } // Client, Tradesperson
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime RegisteredDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public decimal? Rating { get; set; }
    public int ReviewCount { get; set; }
}

public class PaymentManagementDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string BookingDescription { get; set; }
    public decimal Amount { get; set; }
    public int Status { get; set; }
    public int PaymentMethod { get; set; }
    public DateTime ProcessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string TransactionId { get; set; }
}

public class ReportsGeneratorDto
{
    public string ReportType { get; set; } // revenue, users, bookings, etc.
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Format { get; set; } // pdf, excel, csv
}
