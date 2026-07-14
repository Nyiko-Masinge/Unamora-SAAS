using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Bookings;

public class JobTrackingState : BaseEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public TrackingStatus Status { get; set; } = TrackingStatus.EnRoute;
    public decimal? CurrentLatitude { get; set; }
    public decimal? CurrentLongitude { get; set; }
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
}
