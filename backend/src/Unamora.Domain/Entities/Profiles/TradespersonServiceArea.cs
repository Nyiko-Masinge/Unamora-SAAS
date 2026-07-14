using Unamora.Domain.Common;

namespace Unamora.Domain.Entities.Profiles;

public class TradespersonServiceArea : BaseEntity
{
    public Guid TradespersonProfileId { get; set; }
    public TradespersonProfile TradespersonProfile { get; set; } = null!;
    public string AreaName { get; set; } = string.Empty;
    public decimal CenterLatitude { get; set; }
    public decimal CenterLongitude { get; set; }
    public decimal RadiusKm { get; set; }
}
