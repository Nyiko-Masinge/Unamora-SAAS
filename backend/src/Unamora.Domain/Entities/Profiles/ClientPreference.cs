using Unamora.Domain.Common;

namespace Unamora.Domain.Entities.Profiles;

public class ClientPreference : BaseEntity
{
    public Guid ClientProfileId { get; set; }
    public ClientProfile ClientProfile { get; set; } = null!;
    public string PreferredLanguage { get; set; } = "en";
    public decimal MaxDistancePreferenceKm { get; set; } = 25;
    public bool ReceiveNewsletter { get; set; } = true;
    public bool SmsAlertsEnabled { get; set; } = true;
}
