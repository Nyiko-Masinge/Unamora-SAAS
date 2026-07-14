using Unamora.Application.Modules.Chat.DTOs;
using Unamora.Domain.Entities.Chat;

namespace Unamora.Application.Modules.Chat.Services;

public interface IChatService
{
    Task<ChatConversationDto> CreateConversationAsync(CreateConversationDto dto, Guid userId);
    Task<ChatConversationDto> GetConversationAsync(Guid conversationId, Guid userId);
    Task<List<ChatConversationDto>> GetUserConversationsAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
    Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto dto, Guid userId);
    Task<List<ChatMessageDto>> GetConversationMessagesAsync(Guid conversationId, Guid userId, int pageNumber = 1, int pageSize = 50);
    Task<ChatMessageDto> GetMessageAsync(Guid messageId);
    Task MarkMessageAsReadAsync(Guid messageId, Guid userId);
    Task MarkConversationAsReadAsync(Guid conversationId, Guid userId);
    Task UpdateTypingStatusAsync(Guid conversationId, Guid userId, int typingStatus);
    Task<ChatParticipantDto> AddParticipantAsync(Guid conversationId, Guid newParticipantId, Guid adminUserId);
    Task RemoveParticipantAsync(Guid conversationId, Guid participantId, Guid adminUserId);
    Task<List<ChatParticipantDto>> GetConversationParticipantsAsync(Guid conversationId);
    Task ArchiveConversationAsync(Guid conversationId, Guid userId);
    Task DeleteConversationAsync(Guid conversationId, Guid userId);
    Task<int> GetUnreadMessageCountAsync(Guid userId);
    Task<List<ChatNotificationDto>> GetPendingNotificationsAsync(Guid userId);
    Task MarkNotificationAsReadAsync(Guid notificationId);
    Task SendVoiceMessageAsync(SendVoiceMessageDto dto, Guid userId);
}

public class ChatService : IChatService
{
    // Implementation will be provided with database context
    public Task<ChatConversationDto> CreateConversationAsync(CreateConversationDto dto, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<ChatConversationDto> GetConversationAsync(Guid conversationId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ChatConversationDto>> GetUserConversationsAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    public Task<ChatMessageDto> SendMessageAsync(CreateChatMessageDto dto, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ChatMessageDto>> GetConversationMessagesAsync(Guid conversationId, Guid userId, int pageNumber = 1, int pageSize = 50)
    {
        throw new NotImplementedException();
    }

    public Task<ChatMessageDto> GetMessageAsync(Guid messageId)
    {
        throw new NotImplementedException();
    }

    public Task MarkMessageAsReadAsync(Guid messageId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task MarkConversationAsReadAsync(Guid conversationId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateTypingStatusAsync(Guid conversationId, Guid userId, int typingStatus)
    {
        throw new NotImplementedException();
    }

    public Task<ChatParticipantDto> AddParticipantAsync(Guid conversationId, Guid newParticipantId, Guid adminUserId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveParticipantAsync(Guid conversationId, Guid participantId, Guid adminUserId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ChatParticipantDto>> GetConversationParticipantsAsync(Guid conversationId)
    {
        throw new NotImplementedException();
    }

    public Task ArchiveConversationAsync(Guid conversationId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteConversationAsync(Guid conversationId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetUnreadMessageCountAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<ChatNotificationDto>> GetPendingNotificationsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task MarkNotificationAsReadAsync(Guid notificationId)
    {
        throw new NotImplementedException();
    }

    public Task SendVoiceMessageAsync(SendVoiceMessageDto dto, Guid userId)
    {
        throw new NotImplementedException();
    }
}
