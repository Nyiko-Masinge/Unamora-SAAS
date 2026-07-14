using System.Security.Claims;
using Unamora.Application.Common.Interfaces;

namespace Unamora.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public const string SeededClientIdString = "a1f8dce2-1402-4a3c-80e2-64c28ea41e0f";
    public const string SeededTradespersonIdString = "b2e7cba1-3401-4d3b-90f3-75d39fb52e1a";

    public Guid? UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdClaim, out var parsedId))
            {
                return parsedId;
            }

            // Fallback for API testing: check a custom header 'X-User-Id'
            var headerUserId = httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].ToString();
            if (Guid.TryParse(headerUserId, out var parsedHeaderId))
            {
                return parsedHeaderId;
            }

            // Fallback to default client
            return Guid.Parse(SeededClientIdString);
        }
    }

    public string? Email =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) 
        ?? httpContextAccessor.HttpContext?.Request.Headers["X-User-Email"].ToString() 
        ?? "client@unamora.com";

    public bool IsAuthenticated => UserId.HasValue;

    public IEnumerable<string> Roles =>
        httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(c => c.Value)
        ?? new[] { httpContextAccessor.HttpContext?.Request.Headers["X-User-Role"].ToString() ?? "Client" };

    public IEnumerable<string> Permissions =>
        httpContextAccessor.HttpContext?.User?.FindAll("permission").Select(c => c.Value) ?? Array.Empty<string>();
}
