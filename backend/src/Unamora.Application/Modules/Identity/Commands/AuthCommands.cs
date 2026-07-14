using FluentValidation;
using MediatR;
using Unamora.Application.Common.Models;

namespace Unamora.Application.Modules.Identity.Commands;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role,
    string? PhoneNumber) : IRequest<AuthResponse>;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Role).NotEmpty();
    }
}

public record LoginCommand(string Email, string Password, string? DeviceInfo, string? IpAddress) : IRequest<AuthResponse>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public record RefreshTokenCommand(string RefreshToken, string? DeviceInfo, string? IpAddress) : IRequest<AuthResponse>;

public record ForgotPasswordCommand(string Email) : IRequest<bool>;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<bool>;

public record VerifyEmailCommand(string Email, string Token) : IRequest<bool>;

public record SendPhoneOtpCommand(string PhoneNumber) : IRequest<bool>;

public record VerifyPhoneCommand(string PhoneNumber, string Otp) : IRequest<bool>;

public record ExternalLoginCommand(string Provider, string ExternalId, string Email, string FirstName, string LastName, string Role) : IRequest<AuthResponse>;

public record LogoutCommand(Guid UserId, string? RefreshToken) : IRequest<bool>;
