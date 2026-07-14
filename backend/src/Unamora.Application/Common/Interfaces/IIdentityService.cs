using Unamora.Application.Common.Models;

namespace Unamora.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<AuthResponse> RegisterAsync(string email, string password, string firstName, string lastName, string role, string? phoneNumber, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(string email, string password, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken = default);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken = default);
    Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);
    Task<bool> VerifyEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task<bool> SendPhoneOtpAsync(Guid userId, string phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> VerifyPhoneAsync(Guid userId, string phoneNumber, string otp, CancellationToken cancellationToken = default);
    Task<AuthResponse> ExternalLoginAsync(string provider, string externalId, string email, string firstName, string lastName, string role, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(Guid userId, string? refreshToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
