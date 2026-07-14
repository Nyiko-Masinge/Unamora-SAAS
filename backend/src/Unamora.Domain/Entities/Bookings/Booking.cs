using Unamora.Domain.Common;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Bookings;

public class Booking : BaseEntity
{
    public Guid JobRequestId { get; set; }
    public JobRequest JobRequest { get; set; } = null!;
    public Guid? QuoteId { get; set; }
    public Quote? Quote { get; set; }
    
    public Guid ClientProfileId { get; set; }
    public ClientProfile ClientProfile { get; set; } = null!;
    
    public Guid TradespersonProfileId { get; set; }
    public TradespersonProfile TradespersonProfile { get; set; } = null!;
    
    public DateTime ScheduledDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public decimal AgreedHourlyRate { get; set; }
    public decimal EstimatedCost { get; set; }
    public decimal? TotalPaid { get; set; }
    
    public string? CancellationReason { get; set; }
    public Guid? CancelledBy { get; set; }
    
    public ICollection<JobEventLog> EventLogs { get; set; } = new List<JobEventLog>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public JobTrackingState? TrackingState { get; set; }
}
