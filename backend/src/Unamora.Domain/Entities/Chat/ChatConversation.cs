using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Chat;

public class ChatConversation : BaseEntity
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public Guid InitiatorUserId { get; set; }
    public Guid? OtherUserId { get; set; }
    public ConversationStatus Status { get; set; } = ConversationStatus.Active;
    public DateTime LastMessageAt { get; set; }
    public bool IsGroupChat { get; set; } = false;
    
    public ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
