using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Chat;

public class ChatParticipant : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public bool IsActive { get; set; } = true;
    public TypingStatus TypingStatus { get; set; } = TypingStatus.NotTyping;
    public DateTime? TypingStatusUpdatedAt { get; set; }
    
    public ChatConversation Conversation { get; set; }
}

public class ChatMessageAttachment : BaseEntity
{
    public Guid MessageId { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; }
    public long FileSizeBytes { get; set; }
    public MessageType AttachmentType { get; set; }
    
    public ChatMessage Message { get; set; }
}

public class MessageReadReceipt : BaseEntity
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ReadAt { get; set; }
    
    public ChatMessage Message { get; set; }
}

public class ChatNotification : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ConversationId { get; set; }
    public Guid? MessageId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    
    public ChatConversation Conversation { get; set; }
    public ChatMessage? Message { get; set; }
}
