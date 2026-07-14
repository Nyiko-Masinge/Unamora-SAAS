using FluentValidation;
using MediatR;
using Unamora.Domain.Enums;

namespace Unamora.Application.Modules.Verification.Commands;

public record SubmitVerificationCommand(Guid TradespersonProfileId) : IRequest<Guid>;

public record UploadVerificationDocumentCommand(
    Guid VerificationRequestId,
    VerificationDocumentType DocumentType,
    string FileName,
    string MimeType,
    long FileSizeBytes,
    Stream FileStream,
    DateTime? ExpiryDate) : IRequest<Guid>;

public record ReviewVerificationCommand(
    Guid VerificationRequestId,
    bool Approve,
    string? RejectionReason,
    string? Notes) : IRequest<bool>;

public record GetVerificationStatusQuery(Guid TradespersonProfileId) : IRequest<VerificationStatusDto?>;
public record GetPendingVerificationsQuery(int Page = 1, int PageSize = 20) : IRequest<VerificationListDto>;

public class VerificationStatusDto
{
    public Guid RequestId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }
    public IReadOnlyList<VerificationDocumentDto> Documents { get; set; } = Array.Empty<VerificationDocumentDto>();
}

public class VerificationDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ScanStatus { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
}

public class VerificationListDto
{
    public IReadOnlyList<VerificationQueueItemDto> Items { get; set; } = Array.Empty<VerificationQueueItemDto>();
    public int TotalCount { get; set; }
}

public class VerificationQueueItemDto
{
    public Guid RequestId { get; set; }
    public Guid TradespersonProfileId { get; set; }
    public string? BusinessName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public int DocumentCount { get; set; }
}

public class ReviewVerificationCommandValidator : AbstractValidator<ReviewVerificationCommand>
{
    public ReviewVerificationCommandValidator()
    {
        RuleFor(x => x.VerificationRequestId).NotEmpty();
        RuleFor(x => x.RejectionReason).NotEmpty().When(x => !x.Approve);
    }
}
