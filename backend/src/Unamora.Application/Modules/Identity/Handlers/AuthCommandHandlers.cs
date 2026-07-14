using MediatR;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Common.Models;
using Unamora.Application.Modules.Identity.Commands;

namespace Unamora.Application.Modules.Identity.Handlers;

public class RegisterCommandHandler(IIdentityService identityService) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken) =>
        identityService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName, request.Role, request.PhoneNumber, cancellationToken);
}

public class LoginCommandHandler(IIdentityService identityService) : IRequestHandler<LoginCommand, AuthResponse>
{
    public Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken) =>
        identityService.LoginAsync(request.Email, request.Password, request.DeviceInfo, request.IpAddress, cancellationToken);
}

public class RefreshTokenCommandHandler(IIdentityService identityService) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) =>
        identityService.RefreshTokenAsync(request.RefreshToken, request.DeviceInfo, request.IpAddress, cancellationToken);
}

public class ForgotPasswordCommandHandler(IIdentityService identityService) : IRequestHandler<ForgotPasswordCommand, bool>
{
    public Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken) =>
        identityService.ForgotPasswordAsync(request.Email, cancellationToken);
}

public class ResetPasswordCommandHandler(IIdentityService identityService) : IRequestHandler<ResetPasswordCommand, bool>
{
    public Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken) =>
        identityService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword, cancellationToken);
}

public class VerifyEmailCommandHandler(IIdentityService identityService) : IRequestHandler<VerifyEmailCommand, bool>
{
    public Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken) =>
        identityService.VerifyEmailAsync(request.Email, request.Token, cancellationToken);
}

public class ExternalLoginCommandHandler(IIdentityService identityService) : IRequestHandler<ExternalLoginCommand, AuthResponse>
{
    public Task<AuthResponse> Handle(ExternalLoginCommand request, CancellationToken cancellationToken) =>
        identityService.ExternalLoginAsync(request.Provider, request.ExternalId, request.Email, request.FirstName, request.LastName, request.Role, cancellationToken);
}

public class LogoutCommandHandler(IIdentityService identityService) : IRequestHandler<LogoutCommand, bool>
{
    public Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken) =>
        identityService.LogoutAsync(request.UserId, request.RefreshToken, cancellationToken);
}
