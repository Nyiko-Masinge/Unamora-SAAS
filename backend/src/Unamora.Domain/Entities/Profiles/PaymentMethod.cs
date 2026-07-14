using Unamora.Domain.Common;

namespace Unamora.Domain.Entities.Profiles;

public class PaymentMethod : BaseEntity
{
    public Guid ClientProfileId { get; set; }
    public ClientProfile ClientProfile { get; set; } = null!;
    public string CardBrand { get; set; } = string.Empty; // e.g. Visa, Mastercard
    public string LastFour { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string GatewayToken { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
