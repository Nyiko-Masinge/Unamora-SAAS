import React from 'react';
import './Card.css';

interface CardProps extends React.HTMLAttributes<HTMLDivElement> {
  variant?: 'default' | 'elevated' | 'outlined';
  padding?: 'small' | 'medium' | 'large';
}

const Card: React.FC<CardProps> = ({
  variant = 'default',
  padding = 'medium',
  className = '',
  children,
  ...props
}) => {
  return (
    <div
      className={`card card-${variant} card-p-${padding} ${className}`}
      {...props}
    >
      {children}
    </div>
  );
};

export default Card;
