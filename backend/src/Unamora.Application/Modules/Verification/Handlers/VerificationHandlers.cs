using MediatR;
using Unamora.Application.Common.Exceptions;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Modules.Verification.Commands;
using Unamora.Domain.Entities.Verification;
using Unamora.Domain.Enums;

namespace Unamora.Application.Modules.Verification.Handlers;

public class SubmitVerificationCommandHandler(
    IVerificationRepository verificationRepo, ITradespersonProfileRepository profileRepo, ICurrentUserService currentUser, IUnitOfWork uow)
    : IRequestHandler<SubmitVerificationCommand, Guid>
{
    public async Task<Guid> Handle(SubmitVerificationCommand request, CancellationToken cancellationToken)
    {
        var profile = await profileRepo.GetByIdAsync(request.TradespersonProfileId, cancellationToken)
            ?? throw new NotFoundException("Profile not found");
        if (profile.UserId != currentUser.UserId) throw new ForbiddenException("Access denied");

        var existing = await verificationRepo.GetLatestByProfileIdAsync(profile.Id, cancellationToken);
        if (existing is not null && existing.Status is TradespersonVerificationStatus.Submitted or TradespersonVerificationStatus.InReview)
            throw new ConflictException("Verification already in progress");

        var verification = new VerificationRequest
        {
            TradespersonProfileId = profile.Id,
            Status = TradespersonVerificationStatus.Submitted,
            SubmittedAt = DateTime.UtcNow
        };
        profile.VerificationStatus = TradespersonVerificationStatus.Submitted;
        await verificationRepo.AddAsync(verification, cancellationToken);
        profileRepo.Update(profile);
        await uow.SaveChangesAsync(cancellationToken);
        return verification.Id;
    }
}

public class UploadVerificationDocumentCommandHandler(
    IVerificationRepository verificationRepo, IFileStorageService fileStorage, IOcrService ocr, IUnitOfWork uow)
    : IRequestHandler<UploadVerificationDocumentCommand, Guid>
{
    public async Task<Guid> Handle(UploadVerificationDocumentCommand request, CancellationToken cancellationToken)
    {
        var verification = await verificationRepo.GetByIdAsync(request.VerificationRequestId, cancellationToken)
            ?? throw new NotFoundException("Verification request not found");

        var blobUrl = await fileStorage.UploadAsync(request.FileStream, request.FileName, request.MimeType, "verification-documents", cancellationToken);

        var document = new VerificationDocument
        {
            VerificationRequestId = verification.Id,
            DocumentType = request.DocumentType,
            BlobUrl = blobUrl,
            FileName = request.FileName,
            FileSizeBytes = request.FileSizeBytes,
            MimeType = request.MimeType,
            ExpiryDate = request.ExpiryDate,
            ScanStatus = DocumentScanStatus.Processing
        };

        verification.Documents.Add(document);
        verification.Status = TradespersonVerificationStatus.InReview;
        verificationRepo.Update(verification);
        await uow.SaveChangesAsync(cancellationToken);

        var ocrResult = await ocr.ProcessDocumentAsync(blobUrl, cancellationToken);
        document.OcrExtractedText = ocrResult.ExtractedText;
        document.OcrExtractedDataJson = ocrResult.ExtractedDataJson;
        document.ScanStatus = ocrResult.Success ? DocumentScanStatus.Clean : DocumentScanStatus.Failed;
        await uow.SaveChangesAsync(cancellationToken);

        return document.Id;
    }
}

public class ReviewVerificationCommandHandler(
    IVerificationRepository verificationRepo, ITradespersonProfileRepository profileRepo, ICurrentUserService currentUser, IUnitOfWork uow)
    : IRequestHandler<ReviewVerificationCommand, bool>
{
    public async Task<bool> Handle(ReviewVerificationCommand request, CancellationToken cancellationToken)
    {
        var verification = await verificationRepo.GetByIdAsync(request.VerificationRequestId, cancellationToken)
            ?? throw new NotFoundException("Verification request not found");

        var profile = await profileRepo.GetByIdAsync(verification.TradespersonProfileId, cancellationToken)
            ?? throw new NotFoundException("Profile not found");

        verification.ReviewedAt = DateTime.UtcNow;
        verification.ReviewedByUserId = currentUser.UserId;
        verification.Notes = request.Notes;

        if (request.Approve)
        {
            verification.Status = TradespersonVerificationStatus.Approved;
            profile.VerificationStatus = TradespersonVerificationStatus.Approved;
            profile.VerifiedAt = DateTime.UtcNow;
        }
        else
        {
            verification.Status = TradespersonVerificationStatus.Rejected;
            verification.RejectionReason = request.RejectionReason;
            profile.VerificationStatus = TradespersonVerificationStatus.Rejected;
        }

        verification.ReviewLogs.Add(new VerificationReviewLog
        {
            Action = request.Approve ? "Approved" : "Rejected",
            PerformedByUserId = currentUser.UserId ?? Guid.Empty,
            Notes = request.Notes
        });

        verificationRepo.Update(verification);
        profileRepo.Update(profile);
        await uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class GetVerificationStatusQueryHandler(IVerificationRepository verificationRepo) : IRequestHandler<GetVerificationStatusQuery, VerificationStatusDto?>
{
    public async Task<VerificationStatusDto?> Handle(GetVerificationStatusQuery request, CancellationToken cancellationToken)
    {
        var v = await verificationRepo.GetLatestByProfileIdAsync(request.TradespersonProfileId, cancellationToken);
        if (v is null) return null;
        return new VerificationStatusDto
        {
            RequestId = v.Id,
            Status = v.Status.ToString(),
            SubmittedAt = v.SubmittedAt,
            ReviewedAt = v.ReviewedAt,
            RejectionReason = v.RejectionReason,
            Documents = v.Documents.Select(d => new VerificationDocumentDto
            {
                Id = d.Id, DocumentType = d.DocumentType.ToString(), FileName = d.FileName,
                ScanStatus = d.ScanStatus.ToString(), ExpiryDate = d.ExpiryDate
            }).ToList()
        };
    }
}

public class GetPendingVerificationsQueryHandler(IVerificationRepository verificationRepo) : IRequestHandler<GetPendingVerificationsQuery, VerificationListDto>
{
    public async Task<VerificationListDto> Handle(GetPendingVerificationsQuery request, CancellationToken cancellationToken)
    {
        var items = await verificationRepo.GetPendingAsync(cancellationToken);
        var paged = items.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return new VerificationListDto
        {
            TotalCount = items.Count,
            Items = paged.Select(v => new VerificationQueueItemDto
            {
                RequestId = v.Id,
                TradespersonProfileId = v.TradespersonProfileId,
                BusinessName = v.TradespersonProfile?.BusinessName,
                Status = v.Status.ToString(),
                SubmittedAt = v.SubmittedAt,
                DocumentCount = v.Documents.Count
            }).ToList()
        };
    }
}
