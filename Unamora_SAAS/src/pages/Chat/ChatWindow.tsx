import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import './ChatWindow.css';

interface ChatMessage {
  id: string;
  senderId: string;
  senderName: string;
  content: string;
  messageType: number;
  sentAt: string;
  readAt?: string;
  attachments?: ChatAttachment[];
}

interface ChatAttachment {
  id: string;
  fileName: string;
  fileUrl: string;
  fileType: string;
}

export const ChatWindow: React.FC = () => {
  const { conversationId } = useParams<{ conversationId: string }>();
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [newMessage, setNewMessage] = useState('');
  const [isTyping, setIsTyping] = useState(false);
  const [otherUserTyping, setOtherUserTyping] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Fetch messages
    fetchMessages();
    // Setup real-time updates via SignalR
    setupSignalR();
  }, [conversationId]);

  const fetchMessages = async () => {
    try {
      const response = await fetch(`/api/chat/conversations/${conversationId}/messages`);
      const data = await response.json();
      setMessages(data);
      setLoading(false);
    } catch (error) {
      console.error('Failed to fetch messages:', error);
      setLoading(false);
    }
  };

  const setupSignalR = () => {
    // SignalR connection setup for real-time chat
    console.log('Setting up SignalR for real-time messaging');
  };

  const sendMessage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newMessage.trim()) return;

    try {
      const response = await fetch('/api/chat/messages', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          conversationId,
          content: newMessage,
          messageType: 0, // Text
        }),
      });

      if (response.ok) {
        const message = await response.json();
        setMessages([...messages, message]);
        setNewMessage('');
      }
    } catch (error) {
      console.error('Failed to send message:', error);
    }
  };

  const handleTyping = async () => {
    setIsTyping(true);
    try {
      await fetch('/api/chat/typing-status', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          conversationId,
          typingStatus: 1, // Typing
        }),
      });
    } catch (error) {
      console.error('Failed to update typing status:', error);
    }

    setTimeout(() => {
      setIsTyping(false);
    }, 3000);
  };

  const uploadFile = async (file: File) => {
    // File upload implementation
    console.log('Uploading file:', file.name);
  };

  if (loading) return <div className="chat-window loading">Loading messages...</div>;

  return (
    <div className="chat-window">
      <div className="messages-container">
        {messages.map((msg) => (
          <div key={msg.id} className={`message ${msg.senderId === 'currentUserId' ? 'own' : 'other'}`}>
            <div className="message-header">
              <span className="sender-name">{msg.senderName}</span>
              <span className="message-time">
                {new Date(msg.sentAt).toLocaleTimeString()}
              </span>
            </div>
            <div className="message-content">{msg.content}</div>
            {msg.attachments && msg.attachments.length > 0 && (
              <div className="attachments">
                {msg.attachments.map((att) => (
                  <a
                    key={att.id}
                    href={att.fileUrl}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="attachment"
                  >
                    📎 {att.fileName}
                  </a>
                ))}
              </div>
            )}
            {msg.readAt && <span className="read-indicator">✓✓</span>}
          </div>
        ))}
        {otherUserTyping && <div className="typing-indicator">Someone is typing...</div>}
      </div>

      <form onSubmit={sendMessage} className="message-input-form">
        <input
          type="text"
          value={newMessage}
          onChange={(e) => {
            setNewMessage(e.target.value);
            handleTyping();
          }}
          placeholder="Type your message..."
          className="message-input"
        />
        <button type="button" className="attachment-btn" title="Add attachment">
          📎
        </button>
        <button type="submit" className="send-btn">
          Send
        </button>
      </form>
    </div>
  );
};
