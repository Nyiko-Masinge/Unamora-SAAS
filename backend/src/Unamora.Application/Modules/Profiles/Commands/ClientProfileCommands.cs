using MediatR;

namespace Unamora.Application.Modules.Profiles.Commands;

// Profile
public record GetClientProfileQuery() : IRequest<ClientProfileDto?>;
public record UpdateClientProfileCommand(
    string? DefaultAddressLine1,
    string? DefaultAddressLine2,
    string? City,
    string? Province,
    string? PostalCode,
    decimal? DefaultLatitude,
    decimal? DefaultLongitude) : IRequest<bool>;

// Addresses
public record GetAddressesQuery() : IRequest<IReadOnlyList<AddressDto>>;
public record AddAddressCommand(
    string Label,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string Province,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    bool IsDefault) : IRequest<Guid>;
public record DeleteAddressCommand(Guid AddressId) : IRequest<bool>;

// Payment Methods
public record GetPaymentMethodsQuery() : IRequest<IReadOnlyList<PaymentMethodDto>>;
public record AddPaymentMethodCommand(
    string CardBrand,
    string LastFour,
    int ExpiryMonth,
    int ExpiryYear,
    string GatewayToken,
    bool IsDefault) : IRequest<Guid>;
public record DeletePaymentMethodCommand(Guid PaymentMethodId) : IRequest<bool>;

// Preferences
public record GetClientPreferenceQuery() : IRequest<ClientPreferenceDto>;
public record UpdateClientPreferenceCommand(
    string PreferredLanguage,
    decimal MaxDistancePreferenceKm,
    bool ReceiveNewsletter,
    bool SmsAlertsEnabled) : IRequest<bool>;

// Saved & History
public record GetSavedTradespersonsQuery() : IRequest<IReadOnlyList<SavedTradespersonDto>>;
public record ToggleSaveTradespersonCommand(Guid TradespersonProfileId) : IRequest<bool>;

public record GetRecentlyViewedTradespersonsQuery() : IRequest<IReadOnlyList<RecentlyViewedDto>>;
public record AddRecentlyViewedTradespersonCommand(Guid TradespersonProfileId) : IRequest<bool>;
public record GetRecommendationsQuery() : IRequest<IReadOnlyList<RecommendedTradespersonDto>>;

// DTOs
public class ClientProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? DefaultAddressLine1 { get; set; }
    public string? DefaultAddressLine2 { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? PostalCode { get; set; }
    public decimal? DefaultLatitude { get; set; }
    public decimal? DefaultLongitude { get; set; }
    public int TotalJobsPosted { get; set; }
    public decimal? AverageClientRating { get; set; }
}

public class AddressDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsDefault { get; set; }
}

public class PaymentMethodDto
{
    public Guid Id { get; set; }
    public string CardBrand { get; set; } = string.Empty;
    public string LastFour { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public bool IsDefault { get; set; }
}

public class ClientPreferenceDto
{
    public string PreferredLanguage { get; set; } = "en";
    public decimal MaxDistancePreferenceKm { get; set; } = 25;
    public bool ReceiveNewsletter { get; set; } = true;
    public bool SmsAlertsEnabled { get; set; } = true;
}

public class SavedTradespersonDto
{
    public Guid TradespersonProfileId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string TradespersonName { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int CompletedJobs { get; set; }
    public DateTime SavedAt { get; set; }
}

public class RecentlyViewedDto
{
    public Guid TradespersonProfileId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public DateTime ViewedAt { get; set; }
}

public class RecommendedTradespersonDto
{
    public Guid TradespersonProfileId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string TradespersonName { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
