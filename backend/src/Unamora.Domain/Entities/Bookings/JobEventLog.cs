using Unamora.Domain.Common;

namespace Unamora.Domain.Entities.Bookings;

public class JobEventLog : BaseEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public string EventName { get; set; } = string.Empty; // e.g. "BookingCreated", "QuoteAccepted", "EnRoute"
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
