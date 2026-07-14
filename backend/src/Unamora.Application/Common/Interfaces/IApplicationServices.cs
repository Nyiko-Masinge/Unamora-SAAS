namespace Unamora.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IEnumerable<string> Roles { get; }
    IEnumerable<string> Permissions { get; }
}

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions, Guid? sessionId = null);
    string GenerateRefreshToken();
    string HashToken(string token);
}

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string token, CancellationToken cancellationToken = default);
    Task SendPasswordResetAsync(string email, string token, CancellationToken cancellationToken = default);
}

public interface ISmsService
{
    Task SendPhoneOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default);
}

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, string container, CancellationToken cancellationToken = default);
    Task<string> GetSasUploadUrlAsync(string fileName, string container, CancellationToken cancellationToken = default);
    Task DeleteAsync(string blobUrl, CancellationToken cancellationToken = default);
}

public interface IOcrService
{
    Task<OcrResult> ProcessDocumentAsync(string blobUrl, CancellationToken cancellationToken = default);
}

public record OcrResult(string? ExtractedText, string? ExtractedDataJson, bool Success, string? ErrorMessage);

public class MatchingResultDto
{
    public Guid TradespersonId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string TradespersonName { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public decimal MatchScore { get; set; }
    public decimal Rating { get; set; }
    public int CompletedJobs { get; set; }
    public decimal HourlyRateMin { get; set; }
    public decimal HourlyRateMax { get; set; }
    public string AvailabilityStatus { get; set; } = string.Empty;
    public int ResponseTimeMinutes { get; set; }
    public bool IsVerified { get; set; }
    public decimal DistanceScore { get; set; }
    public decimal AvailabilityScore { get; set; }
    public decimal TrustScore { get; set; }
    public decimal ExperienceScore { get; set; }
    public decimal PriceScore { get; set; }
    public decimal ReviewsScore { get; set; }
    public decimal ReliabilityScore { get; set; }
}

public interface IMatchingService
{
    Task<IEnumerable<MatchingResultDto>> FindMatchesAsync(Guid jobRequestId, CancellationToken cancellationToken = default);
}

