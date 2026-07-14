using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Portfolio;

public class PortfolioMedia : BaseEntity
{
    public Guid PortfolioProjectId { get; set; }
    public PortfolioProject PortfolioProject { get; set; } = null!;
    public PortfolioMediaType MediaType { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
}
