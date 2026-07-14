using Unamora.Domain.Common;
using Unamora.Domain.Entities.Catalog;
using Unamora.Domain.Entities.Profiles;

namespace Unamora.Domain.Entities.Portfolio;

public class PortfolioProject : BaseEntity
{
    public Guid TradespersonProfileId { get; set; }
    public TradespersonProfile TradespersonProfile { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ServiceCategoryId { get; set; }
    public ServiceCategory? ServiceCategory { get; set; }
    public string? ClientName { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsFeatured { get; set; }
    public ICollection<PortfolioMedia> Media { get; set; } = new List<PortfolioMedia>();
}
