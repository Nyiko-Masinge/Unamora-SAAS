using Unamora.Domain.Common;

namespace Unamora.Domain.Entities.Profiles;

public class RecentlyViewedTradesperson : BaseEntity
{
    public Guid ClientProfileId { get; set; }
    public ClientProfile ClientProfile { get; set; } = null!;
    public Guid TradespersonProfileId { get; set; }
    public TradespersonProfile TradespersonProfile { get; set; } = null!;
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
}
