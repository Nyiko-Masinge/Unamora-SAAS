using Unamora.Domain.Common;
using Unamora.Domain.Enums;

namespace Unamora.Domain.Entities.Chat;

public class ChatMessage : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; }
    public MessageType MessageType { get; set; } = MessageType.Text;
    public MessageStatus Status { get; set; } = MessageStatus.Sent;
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    
    public ChatConversation Conversation { get; set; }
    public ChatMessage? ReplyToMessage { get; set; }
    public ICollection<ChatMessageAttachment> Attachments { get; set; } = new List<ChatMessageAttachment>();
    public ICollection<MessageReadReceipt> ReadReceipts { get; set; } = new List<MessageReadReceipt>();
}
