import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './ChatList.css';

interface Conversation {
  id: string;
  title: string;
  lastMessage?: string;
  lastMessageAt: string;
  unreadMessageCount: number;
  participants: number;
  participantCount: number;
}

export const ChatList: React.FC = () => {
  const [conversations, setConversations] = useState<Conversation[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    fetchConversations();
  }, []);

  const fetchConversations = async () => {
    try {
      const response = await fetch('/api/chat/conversations');
      const data = await response.json();
      setConversations(data);
      setLoading(false);
    } catch (error) {
      console.error('Failed to fetch conversations:', error);
      setLoading(false);
    }
  };

  const filteredConversations = conversations.filter((conv) =>
    conv.title.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSelectConversation = (conversationId: string) => {
    navigate(`/chat/${conversationId}`);
  };

  const handleStartNewChat = () => {
    navigate('/chat/new');
  };

  if (loading) return <div className="chat-list loading">Loading conversations...</div>;

  return (
    <div className="chat-list">
      <div className="chat-header">
        <h2>Messages</h2>
        <button onClick={handleStartNewChat} className="new-chat-btn">
          + New Chat
        </button>
      </div>

      <div className="search-box">
        <input
          type="text"
          placeholder="Search conversations..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
      </div>

      <div className="conversations">
        {filteredConversations.length === 0 ? (
          <div className="no-conversations">No conversations yet</div>
        ) : (
          filteredConversations.map((conversation) => (
            <div
              key={conversation.id}
              className={`conversation-item ${conversation.unreadMessageCount > 0 ? 'unread' : ''}`}
              onClick={() => handleSelectConversation(conversation.id)}
            >
              <div className="conversation-header">
                <h3 className="conversation-title">{conversation.title}</h3>
                {conversation.unreadMessageCount > 0 && (
                  <span className="unread-badge">{conversation.unreadMessageCount}</span>
                )}
              </div>
              <p className="last-message">{conversation.lastMessage || 'No messages yet'}</p>
              <div className="conversation-meta">
                <span className="time">
                  {new Date(conversation.lastMessageAt).toLocaleDateString()}
                </span>
                <span className="participants">{conversation.participantCount} members</span>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};
