namespace Unamora.Domain.Enums;

public enum MessageType
{
    Text = 0,
    Image = 1,
    Video = 2,
    Voice = 3,
    File = 4,
    System = 5
}

public enum MessageStatus
{
    Sent = 0,
    Delivered = 1,
    Read = 2,
    Failed = 3
}

public enum TypingStatus
{
    NotTyping = 0,
    Typing = 1,
    Stopped = 2
}

public enum ConversationStatus
{
    Active = 0,
    Archived = 1,
    Closed = 2
}
