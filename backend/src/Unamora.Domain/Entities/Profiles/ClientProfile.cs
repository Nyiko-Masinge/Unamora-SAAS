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
}
