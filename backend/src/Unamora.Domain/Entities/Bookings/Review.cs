using Unamora.Domain.Common;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Bookings;

public class Review : BaseEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public int Rating { get; set; } // 1-5 stars
    public string? Comment { get; set; }
    public ReviewDirection Direction { get; set; }
}
