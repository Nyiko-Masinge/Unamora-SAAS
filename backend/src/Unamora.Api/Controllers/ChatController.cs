using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unamora.Application.Common.Interfaces;
using Unamora.Application.Modules.Chat.DTOs;
using Unamora.Application.Modules.Chat.Services;

namespace Unamora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ICurrentUserService _currentUserService;

    public ChatController(IChatService chatService, ICurrentUserService currentUserService)
    {
        _chatService = chatService;
        _currentUserService = currentUserService;
    }

    [HttpPost("conversations")]
    public async Task<ActionResult<ChatConversationDto>> CreateConversation([FromBody] CreateConversationDto dto)
    {
        var userId = _currentUserService.UserId ?? Guid.Empty;
        var conversation = await _chatService.CreateConversationAsync(dto, userId);
        return CreatedAtAction(nameof(GetConversation), new { id = conversation.Id }, conversation);
    }

    [HttpGet("conversations/{id}")]
    public async Task<ActionResult<ChatConversationDto>> GetConversation(Guid id)
    {
        var userId = _currentUserService.UserId;
        var conversation = await _chatService.GetConversationAsync(id, userId);
        return Ok(conversation);
    }

    [HttpGet("conversations")]
    public async Task<ActionResult<List<ChatConversationDto>>> GetConversations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = _currentUserService.UserId;
        var conversations = await _chatService.GetUserConversationsAsync(userId, pageNumber, pageSize);
        return Ok(conversations);
    }

    [HttpPost("messages")]
    public async Task<ActionResult<ChatMessageDto>> SendMessage([FromBody] CreateChatMessageDto dto)
    {
        var userId = _currentUserService.UserId;
        var message = await _chatService.SendMessageAsync(dto, userId);
        return CreatedAtAction(nameof(GetMessage), new { id = message.Id }, message);
    }

    [HttpGet("messages/{id}")]
    public async Task<ActionResult<ChatMessageDto>> GetMessage(Guid id)
    {
        var message = await _chatService.GetMessageAsync(id);
        return Ok(message);
    }

    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetConversationMessages(Guid conversationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var userId = _currentUserService.UserId;
        var messages = await _chatService.GetConversationMessagesAsync(conversationId, userId, pageNumber, pageSize);
        return Ok(messages);
    }

    [HttpPut("messages/{id}/read")]
    public async Task<IActionResult> MarkMessageAsRead(Guid id)
    {
        var userId = _currentUserService.UserId;
        await _chatService.MarkMessageAsReadAsync(id, userId);
        return NoContent();
    }

    [HttpPut("conversations/{id}/read")]
    public async Task<IActionResult> MarkConversationAsRead(Guid id)
    {
        var userId = _currentUserService.UserId;
        await _chatService.MarkConversationAsReadAsync(id, userId);
        return NoContent();
    }

    [HttpPost("typing-status")]
    public async Task<IActionResult> UpdateTypingStatus([FromBody] UpdateTypingStatusDto dto)
    {
        var userId = _currentUserService.UserId;
        await _chatService.UpdateTypingStatusAsync(dto.ConversationId, userId, dto.TypingStatus);
        return NoContent();
    }

    [HttpPost("conversations/{id}/participants")]
    public async Task<ActionResult<ChatParticipantDto>> AddParticipant(Guid id, [FromBody] Guid participantId)
    {
        var adminUserId = _currentUserService.UserId;
        var participant = await _chatService.AddParticipantAsync(id, participantId, adminUserId);
        return CreatedAtAction(nameof(GetParticipants), new { conversationId = id }, participant);
    }

    [HttpDelete("conversations/{conversationId}/participants/{participantId}")]
    public async Task<IActionResult> RemoveParticipant(Guid conversationId, Guid participantId)
    {
        var adminUserId = _currentUserService.UserId;
        await _chatService.RemoveParticipantAsync(conversationId, participantId, adminUserId);
        return NoContent();
    }

    [HttpGet("conversations/{id}/participants")]
    public async Task<ActionResult<List<ChatParticipantDto>>> GetParticipants(Guid id)
    {
        var participants = await _chatService.GetConversationParticipantsAsync(id);
        return Ok(participants);
    }

    [HttpPut("conversations/{id}/archive")]
    public async Task<IActionResult> ArchiveConversation(Guid id)
    {
        var userId = _currentUserService.UserId;
        await _chatService.ArchiveConversationAsync(id, userId);
        return NoContent();
    }

    [HttpDelete("conversations/{id}")]
    public async Task<IActionResult> DeleteConversation(Guid id)
    {
        var userId = _currentUserService.UserId;
        await _chatService.DeleteConversationAsync(id, userId);
        return NoContent();
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = _currentUserService.UserId;
        var count = await _chatService.GetUnreadMessageCountAsync(userId);
        return Ok(count);
    }

    [HttpGet("notifications")]
    public async Task<ActionResult<List<ChatNotificationDto>>> GetNotifications()
    {
        var userId = _currentUserService.UserId;
        var notifications = await _chatService.GetPendingNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpPut("notifications/{id}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(Guid id)
    {
        await _chatService.MarkNotificationAsReadAsync(id);
        return NoContent();
    }

    [HttpPost("voice-messages")]
    public async Task<IActionResult> SendVoiceMessage([FromBody] SendVoiceMessageDto dto)
    {
        var userId = _currentUserService.UserId;
        await _chatService.SendVoiceMessageAsync(dto, userId);
        return Ok();
    }
}
