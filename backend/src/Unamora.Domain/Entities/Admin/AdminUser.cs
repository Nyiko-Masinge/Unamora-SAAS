using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Admin;

public class AdminUser : BaseEntity
{
    public Guid UserId { get; set; }
    public AdminRole Role { get; set; }
    public string? Department { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? RemovedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Permissions { get; set; } // JSON string of permissions
    
    public ICollection<AdminAction> Actions { get; set; } = new List<AdminAction>();
}

public class AdminAction : BaseEntity
{
    public Guid AdminUserId { get; set; }
    public string ActionType { get; set; }
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? Changes { get; set; } // JSON string
    public string? Reason { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? IpAddress { get; set; }
    
    public AdminUser AdminUser { get; set; }
}
