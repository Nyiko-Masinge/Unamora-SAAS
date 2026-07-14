using MediatR;

namespace Unamora.Application.Modules.Catalog.Commands;

public record AdvancedSearchQuery(
    string? TradeName,
    int? TradeId,
    decimal? Latitude,
    decimal? Longitude,
    decimal? RadiusKm,
    string? Availability,
    decimal? MinRating,
    decimal? MaxPrice,
    string? Province,
    string? City,
    string? Business,
    string? LanguageCode,
    bool? EmergencyOnly) : IRequest<IReadOnlyList<SearchResultDto>>;

public record AiSearchQuery(string Prompt) : IRequest<IReadOnlyList<SearchResultDto>>;

public class SearchResultDto
{
    public Guid TradespersonId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string TradespersonName { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int CompletedJobs { get; set; }
    public decimal ServiceRadiusKm { get; set; }
    public decimal HourlyRateMin { get; set; }
    public decimal HourlyRateMax { get; set; }
    public string AvailabilityStatus { get; set; } = string.Empty;
    public bool HasPoliceClearance { get; set; }
    public bool IsVerified { get; set; }
    public decimal DistanceKm { get; set; }
    public double MatchScore { get; set; }
}
