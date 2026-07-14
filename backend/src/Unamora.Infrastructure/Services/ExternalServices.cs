using Microsoft.Extensions.Logging;
using Unamora.Application.Common.Interfaces;

namespace Unamora.Infrastructure.Services;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public Task SendEmailVerificationAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Email verification sent to {Email}. Token: {Token}", email, token);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Password reset sent to {Email}. Token: {Token}", email, token);
        return Task.CompletedTask;
    }
}

public class SmsService(ILogger<SmsService> logger) : ISmsService
{
    public Task SendPhoneOtpAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("OTP sent to {Phone}. OTP: {Otp}", phoneNumber, otp);
        return Task.CompletedTask;
    }
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

public class OcrService(ILogger<OcrService> logger) : IOcrService
{
    public Task<OcrResult> ProcessDocumentAsync(string blobUrl, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("OCR processing queued for {BlobUrl}", blobUrl);
        return Task.FromResult(new OcrResult(null, null, true, null));
    }
}
