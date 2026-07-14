using Unamora.Domain.Common;
using Unamora.Domain.Entities.Profiles;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Bookings;

public class Quote : BaseEntity
{
    public Guid JobRequestId { get; set; }
    public JobRequest JobRequest { get; set; } = null!;
    public Guid TradespersonProfileId { get; set; }
    public TradespersonProfile TradespersonProfile { get; set; } = null!;
    public decimal EstimatedHours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal MaterialsCost { get; set; }
    public string? AdditionalNotes { get; set; }
    public QuoteStatus Status { get; set; } = QuoteStatus.Pending;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
