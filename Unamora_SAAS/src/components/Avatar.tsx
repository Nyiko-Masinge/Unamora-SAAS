import React from 'react';
import './Avatar.css';

interface AvatarProps {
  src?: string;
  alt?: string;
  initials?: string;
  size?: 'small' | 'medium' | 'large' | 'xlarge';
  status?: 'online' | 'offline' | 'away';
  className?: string;
}

const Avatar: React.FC<AvatarProps> = ({
  src,
  alt = 'Avatar',
  initials,
  size = 'medium',
  status,
  className = '',
}) => {
  return (
    <div className={`avatar avatar-${size} ${status ? `status-${status}` : ''} ${className}`}>
      {src ? (
        <img src={src} alt={alt} />
      ) : (
        <span className="initials">{initials}</span>
      )}
      {status && <div className="status-indicator" />}
    </div>
  );
};

export default Avatar;
