using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Verification;

public class VerificationDocument : BaseEntity
{
    public Guid VerificationRequestId { get; set; }
    public VerificationRequest VerificationRequest { get; set; } = null!;
    public VerificationDocumentType DocumentType { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public DocumentScanStatus ScanStatus { get; set; } = DocumentScanStatus.Pending;
    public string? OcrExtractedText { get; set; }
    public string? OcrExtractedDataJson { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
