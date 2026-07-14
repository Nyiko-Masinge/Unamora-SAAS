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
