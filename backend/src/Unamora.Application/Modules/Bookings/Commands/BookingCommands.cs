using MediatR;
using Unamora.Domain.Enums;

namespace Unamora.Application.Modules.Bookings.Commands;

// Job Requests
public record CreateJobRequestCommand(
    int TradeId,
    string Description,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string Province,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    DateTime? PreferredDate,
    decimal? BudgetMax) : IRequest<Guid>;

public record GetJobRequestQuery(Guid JobRequestId) : IRequest<JobRequestDto?>;

// Quotes
public record SubmitQuoteCommand(
    Guid JobRequestId,
    Guid TradespersonProfileId,
    decimal EstimatedHours,
    decimal HourlyRate,
    decimal MaterialsCost,
    string? AdditionalNotes) : IRequest<Guid>;


public record AcceptQuoteCommand(Guid QuoteId) : IRequest<Guid>; // Returns BookingId
public record DeclineQuoteCommand(Guid QuoteId) : IRequest<bool>;

// Bookings
public record GetBookingQuery(Guid BookingId) : IRequest<BookingDto?>;
public record GetClientBookingsQuery() : IRequest<IReadOnlyList<BookingDto>>;
public record GetTradespersonBookingsQuery(Guid TradespersonProfileId) : IRequest<IReadOnlyList<BookingDto>>;
public record CompleteBookingCommand(Guid BookingId) : IRequest<bool>;
public record CancelBookingCommand(Guid BookingId, string Reason) : IRequest<bool>;

// Tracking & Timeline
public record GetTrackingStateQuery(Guid BookingId) : IRequest<TrackingStateDto?>;
public record UpdateTrackingStateCommand(
    Guid BookingId,
    TrackingStatus Status,
    decimal? Latitude,
    decimal? Longitude) : IRequest<bool>;

public record GetJobEventLogsQuery(Guid BookingId) : IRequest<IReadOnlyList<JobEventLogDto>>;

// Reviews
public record SubmitReviewCommand(
    Guid BookingId,
    int Rating,
    string? Comment,
    ReviewDirection Direction) : IRequest<Guid>;

// DTOs
public class JobRequestDto
{
    public Guid Id { get; set; }
    public Guid ClientProfileId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime? PreferredDate { get; set; }
    public decimal? BudgetMax { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<QuoteDto> Quotes { get; set; } = new();
}

public class QuoteDto
{
    public Guid Id { get; set; }
    public Guid JobRequestId { get; set; }
    public Guid TradespersonProfileId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string TradespersonName { get; set; } = string.Empty;
    public decimal EstimatedHours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal MaterialsCost { get; set; }
    public decimal EstimatedTotalCost => (EstimatedHours * HourlyRate) + MaterialsCost;
    public string? AdditionalNotes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid JobRequestId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string TradespersonBusinessName { get; set; } = string.Empty;
    public string TradespersonName { get; set; } = string.Empty;
    public string TradeName { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string TimeRange { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal AgreedHourlyRate { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal? TotalPaid { get; set; }
    public List<JobEventLogDto> Timeline { get; set; } = new();
}

public class TrackingStateDto
{
    public Guid BookingId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}

public class JobEventLogDto
{
    public string EventName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
