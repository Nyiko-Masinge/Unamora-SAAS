using Unamora.Domain.Common;

namespace Unamora.Domain.Entities.Profiles;

public class ClientProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string? DefaultAddressLine1 { get; set; }
    public string? DefaultAddressLine2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public string CountryCode { get; set; } = "ZA";
    public decimal? DefaultLatitude { get; set; }
    public decimal? DefaultLongitude { get; set; }
    public int TotalJobsPosted { get; set; }
    public decimal? AverageClientRating { get; set; }

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
    public ClientPreference? Preference { get; set; }
    public ICollection<SavedTradesperson> SavedTradespersons { get; set; } = new List<SavedTradesperson>();
    public ICollection<RecentlyViewedTradesperson> RecentlyViewed { get; set; } = new List<RecentlyViewedTradesperson>();
}

