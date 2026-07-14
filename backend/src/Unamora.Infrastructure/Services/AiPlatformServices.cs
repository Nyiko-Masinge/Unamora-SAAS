using System.Collections.Concurrent;
using Unamora.Application.Common.Interfaces;

namespace Unamora.Infrastructure.Services;

/// <summary>Safe baseline implementations. Provider integrations belong behind these contracts.</summary>
public sealed class AiAssistantService : IAiAssistantService
{
    public Task<AiAssistantResponse> AnswerAsync(AiAssistantRequest request, CancellationToken cancellationToken = default)
    {
        var message = request.Message.Trim();
        var escalation = message.Contains("refund", StringComparison.OrdinalIgnoreCase)
            || message.Contains("dispute", StringComparison.OrdinalIgnoreCase)
            || message.Contains("fraud", StringComparison.OrdinalIgnoreCase);
        var answer = escalation
            ? "I’ve recorded your request for our support team. They can safely review the account and booking details with you."
            : "I can help with bookings, profiles, payments, and platform policies. For account-specific details, please sign in and choose the relevant booking.";
        return Task.FromResult(new AiAssistantResponse(answer, ["Unamora Help Centre"], escalation, Guid.NewGuid().ToString("N")));
    }
}

public sealed class NotificationService : INotificationService
{
    private readonly ConcurrentDictionary<string, NotificationMessage> _queued = new();

    public Task QueueAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        _queued.TryAdd(message.IdempotencyKey, message);
        return Task.CompletedTask;
    }
}

public sealed class FraudRiskService : IFraudRiskService
{
    public Task<FraudRiskAssessment> AssessAsync(FraudRiskRequest request, CancellationToken cancellationToken = default)
    {
        var reasons = new List<string>();
        var score = 0;
        if (request.Signals.TryGetValue("velocity", out var velocity) && int.TryParse(velocity, out var count) && count > 10) { score += 35; reasons.Add("High action velocity"); }
        if (request.Signals.TryGetValue("duplicate", out var duplicate) && duplicate.Equals("true", StringComparison.OrdinalIgnoreCase)) { score += 45; reasons.Add("Potential duplicate identity signal"); }
        if (request.Signals.TryGetValue("locationMismatch", out var mismatch) && mismatch.Equals("true", StringComparison.OrdinalIgnoreCase)) { score += 25; reasons.Add("Location mismatch"); }
        var level = score >= 60 ? "high" : score >= 30 ? "medium" : "low";
        return Task.FromResult(new FraudRiskAssessment(score, level, reasons, score >= 30));
    }
}
