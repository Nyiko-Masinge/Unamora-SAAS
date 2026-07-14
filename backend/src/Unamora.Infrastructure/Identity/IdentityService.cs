using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unamora.Application.Common.Exceptions;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Common.Models;
using Unamora.Domain.Entities.Identity;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Enums;
using Unamora.Infrastructure.Persistence;
using Unamora.Shared.Constants;

namespace Unamora.Infrastructure.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    UnamoraDbContext context,
    ITokenService tokenService,
    IEmailService emailService,
    ISmsService smsService,
    ILogger<IdentityService> logger) : IIdentityService
{
    public async Task<AuthResponse> RegisterAsync(string email, string password, string firstName, string lastName, string role, string? phoneNumber, CancellationToken cancellationToken = default)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null) throw new ConflictException("Email already registered");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            AuthProvider = AuthProvider.Local,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded) throw new ValidationException(result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));

        if (!await roleManager.RoleExistsAsync(role))
            throw new ValidationException("Role", "Invalid role specified");

        await userManager.AddToRoleAsync(user, role);

        if (role == RoleNames.Client)
            await context.ClientProfiles.AddAsync(new ClientProfile { UserId = user.Id }, cancellationToken);
        else if (role == RoleNames.Tradesman)
            await context.TradespersonProfiles.AddAsync(new TradespersonProfile { UserId = user.Id }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        var verifyToken = tokenService.GenerateRefreshToken();
        await context.EmailVerificationTokens.AddAsync(new EmailVerificationToken
        {
            UserId = user.Id,
            TokenHash = tokenService.HashToken(verifyToken),
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await emailService.SendEmailVerificationAsync(email, verifyToken, cancellationToken);

        return await BuildAuthResponseAsync(user, null, null, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(string email, string password, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email) ?? throw new UnauthorizedException("Invalid credentials");
        if (!user.IsActive) throw new UnauthorizedException("Account is disabled");

        var valid = await userManager.CheckPasswordAsync(user, password);
        if (!valid) throw new UnauthorizedException("Invalid credentials");

        user.LastLoginAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        return await BuildAuthResponseAsync(user, deviceInfo, ipAddress, cancellationToken);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var hash = tokenService.HashToken(refreshToken);
        var stored = await context.RefreshTokens.Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == hash && !r.IsRevoked, cancellationToken)
            ?? throw new UnauthorizedException("Invalid refresh token");

        if (stored.ExpiresAt < DateTime.UtcNow)
        {
            stored.IsRevoked = true;
            stored.RevokedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException("Refresh token expired");
        }

        stored.IsRevoked = true;
        stored.RevokedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        return await BuildAuthResponseAsync(stored.User, deviceInfo, ipAddress, cancellationToken);
    }

    public async Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) return true;

        var token = tokenService.GenerateRefreshToken();
        await context.PasswordResetTokens.AddAsync(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = tokenService.HashToken(token),
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await emailService.SendPasswordResetAsync(email, token, cancellationToken);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found");
        var hash = tokenService.HashToken(token);
        var resetToken = await context.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.UserId == user.Id && t.TokenHash == hash && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow, cancellationToken)
            ?? throw new ValidationException("Token", "Invalid or expired reset token");

        var result = await userManager.RemovePasswordAsync(user);
        if (!result.Succeeded && !user.PasswordHash?.StartsWith("AQ") == true)
        {
            result = await userManager.ChangePasswordAsync(user, user.PasswordHash!, newPassword);
        }
        else
        {
            result = await userManager.AddPasswordAsync(user, newPassword);
        }

        if (!result.Succeeded)
        {
            var resetResult = await userManager.ResetPasswordAsync(user, await userManager.GeneratePasswordResetTokenAsync(user), newPassword);
            if (!resetResult.Succeeded) throw new ValidationException("Password", "Could not reset password");
        }

        resetToken.IsUsed = true;
        resetToken.UsedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> VerifyEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found");
        var hash = tokenService.HashToken(token);
        var verifyToken = await context.EmailVerificationTokens
            .FirstOrDefaultAsync(t => t.UserId == user.Id && t.TokenHash == hash && t.Status != VerificationStatus.Verified && t.ExpiresAt > DateTime.UtcNow, cancellationToken)
            ?? throw new ValidationException("Token", "Invalid or expired verification token");

        verifyToken.Status = VerificationStatus.Verified;
        verifyToken.VerifiedAt = DateTime.UtcNow;
        user.IsEmailVerified = true;
        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SendPhoneOtpAsync(Guid userId, string phoneNumber, CancellationToken cancellationToken = default)
    {
        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        await context.PhoneVerificationTokens.AddAsync(new PhoneVerificationToken
        {
            UserId = userId,
            OtpHash = tokenService.HashToken(otp),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await smsService.SendPhoneOtpAsync(phoneNumber, otp, cancellationToken);
        return true;
    }

    public async Task<bool> VerifyPhoneAsync(Guid userId, string phoneNumber, string otp, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User not found");
        var hash = tokenService.HashToken(otp);
        var token = await context.PhoneVerificationTokens
            .Where(t => t.UserId == userId && t.Status != VerificationStatus.Verified && t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new ValidationException("Otp", "Invalid or expired OTP");

        token.AttemptCount++;
        if (token.OtpHash != hash)
        {
            await context.SaveChangesAsync(cancellationToken);
            throw new ValidationException("Otp", "Invalid OTP");
        }

        token.Status = VerificationStatus.Verified;
        token.VerifiedAt = DateTime.UtcNow;
        user.PhoneNumber = phoneNumber;
        user.IsPhoneVerified = true;
        user.PhoneNumberConfirmed = true;
        await userManager.UpdateAsync(user);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<AuthResponse> ExternalLoginAsync(string provider, string externalId, string email, string firstName, string lastName, string role, CancellationToken cancellationToken = default)
    {
        var authProvider = provider.ToLowerInvariant() switch
        {
            "google" => AuthProvider.Google,
            "microsoft" => AuthProvider.Microsoft,
            _ => throw new ValidationException("Provider", "Unsupported provider")
        };

        var user = await context.Users.FirstOrDefaultAsync(u => u.ExternalProviderId == externalId, cancellationToken)
            ?? await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ExternalProviderId = externalId,
                AuthProvider = authProvider,
                IsEmailVerified = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(user);
            await userManager.AddToRoleAsync(user, role);

            if (role == RoleNames.Client)
                await context.ClientProfiles.AddAsync(new ClientProfile { UserId = user.Id }, cancellationToken);
            else if (role == RoleNames.Tradesman)
                await context.TradespersonProfiles.AddAsync(new TradespersonProfile { UserId = user.Id }, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        return await BuildAuthResponseAsync(user, null, null, cancellationToken);
    }

    public async Task<bool> LogoutAsync(Guid userId, string? refreshToken, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(refreshToken))
        {
            var hash = tokenService.HashToken(refreshToken);
            var token = await context.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == userId && r.TokenHash == hash, cancellationToken);
            if (token is not null) { token.IsRevoked = true; token.RevokedAt = DateTime.UtcNow; }
        }
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var roles = await userManager.GetRolesAsync(await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User not found"));
        var roleIds = await context.Roles.Where(r => roles.Contains(r.Name!)).Select(r => r.Id).ToListAsync(cancellationToken);
        return await context.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private async Task<AuthResponse> BuildAuthResponseAsync(ApplicationUser user, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken)
    {
        var roles = await userManager.GetRolesAsync(user);
        var permissions = await GetUserPermissionsAsync(user.Id, cancellationToken);

        var session = new UserSession
        {
            UserId = user.Id,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            LastActivityAt = DateTime.UtcNow
        };
        await context.UserSessions.AddAsync(session, cancellationToken);

        var refreshTokenValue = tokenService.GenerateRefreshToken();
        await context.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenService.HashToken(refreshTokenValue),
            ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_DAYS") ?? "7")),
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            SessionId = session.Id
        }, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var accessToken = tokenService.GenerateAccessToken(user.Id, user.Email!, roles, permissions, session.Id);
        var expiresMinutes = 15;

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiresMinutes),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified,
                Roles = roles.ToList()
            }
        };
    }
}
