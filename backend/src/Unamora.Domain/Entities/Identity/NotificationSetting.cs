using Unamora.Domain.Entities.Identity;

namespace Unamora.Domain.Entities.Identity;

public class NotificationSetting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = true;
    public bool PushEnabled { get; set; } = true;
    public bool MarketingEnabled { get; set; } = false;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
