using Unamora.Domain.Common;
using Unamora.Domain.Entities.Catalog;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Bookings;

public class JobRequest : BaseEntity
{
    public Guid ClientProfileId { get; set; }
    public ClientProfile ClientProfile { get; set; } = null!;
    public int TradeId { get; set; }
    public Trade Trade { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    
    // Request Address Details
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    
    // GPS Coordinates for mapping/matching
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    
    public DateTime? PreferredDate { get; set; }
    public decimal? BudgetMax { get; set; }
    public JobRequestStatus Status { get; set; } = JobRequestStatus.Pending;

    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}
