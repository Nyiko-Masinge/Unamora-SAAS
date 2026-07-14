namespace Unamora.Domain.Enums;

public enum AuthProvider
{
    Local = 0,
    Google = 1,
    Microsoft = 2
}

public enum VerificationStatus
{
    Pending = 0,
    EmailSent = 1,
    Verified = 2,
    Expired = 3
}

public enum SessionStatus
{
    Active = 0,
    Revoked = 1,
    Expired = 2
}
