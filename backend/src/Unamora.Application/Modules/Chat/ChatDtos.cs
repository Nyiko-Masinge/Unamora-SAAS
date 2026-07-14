namespace Unamora.Application.Modules.Chat.DTOs;

public class CreateChatMessageDto
{
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
    public int MessageType { get; set; } = 0;
    public Guid? ReplyToMessageId { get; set; }
    public List<string>? AttachmentUrls { get; set; }
}

public class ChatMessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; }
    public string? SenderProfilePicture { get; set; }
    public string Content { get; set; }
    public int MessageType { get; set; }
    public int Status { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    public List<ChatMessageAttachmentDto>? Attachments { get; set; }
    public List<Guid>? ReadByUserIds { get; set; }
}

public class ChatMessageAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public string FileType { get; set; }
    public long FileSizeBytes { get; set; }
}

public class CreateConversationDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public Guid? OtherUserId { get; set; }
    public List<Guid>? ParticipantUserIds { get; set; }
    public bool IsGroupChat { get; set; } = false;
}

public class ChatConversationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public Guid InitiatorUserId { get; set; }
    public int Status { get; set; }
    public DateTime LastMessageAt { get; set; }
    public bool IsGroupChat { get; set; }
    public int ParticipantCount { get; set; }
    public ChatMessageDto? LastMessage { get; set; }
    public List<ChatParticipantDto>? Participants { get; set; }
    public int UnreadMessageCount { get; set; }
}

public class ChatParticipantDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; }
    public int TypingStatus { get; set; }
}

public class UpdateTypingStatusDto
{
    public Guid ConversationId { get; set; }
    public int TypingStatus { get; set; } // 0 = NotTyping, 1 = Typing, 2 = Stopped
}

public class MarkMessageAsReadDto
{
    public Guid MessageId { get; set; }
    public DateTime ReadAt { get; set; }
}

public class ChatNotificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ConversationId { get; set; }
    public Guid? MessageId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SendVoiceMessageDto
{
    public Guid ConversationId { get; set; }
    public string AudioFileUrl { get; set; }
    public int DurationSeconds { get; set; }
}
