using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Common.Interfaces;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/ai")]
public sealed class AiController(IAiAssistantService assistant, IFraudRiskService fraudRisk, ICurrentUserService currentUser) : ControllerBase
{
    [HttpPost("chat")]
    [AllowAnonymous]
    public Task<AiAssistantResponse> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken) =>
        assistant.AnswerAsync(new AiAssistantRequest(request.Message, request.ConversationId, currentUser.UserId), cancellationToken);

    [HttpPost("fraud/assess")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public Task<FraudRiskAssessment> AssessFraud([FromBody] FraudRiskRequest request, CancellationToken cancellationToken) =>
        fraudRisk.AssessAsync(request, cancellationToken);
}

public record ChatRequest(string Message, string? ConversationId);
