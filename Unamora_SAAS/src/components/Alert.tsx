import React from 'react';
import './Alert.css';

interface AlertProps {
  message: string;
  type?: 'error' | 'success' | 'warning' | 'info';
  onClose?: () => void;
  autoClose?: number;
  dismissible?: boolean;
  className?: string;
}

const Alert: React.FC<AlertProps> = ({
  message,
  type = 'info',
  onClose,
  autoClose,
  dismissible = true,
  className = '',
}) => {
  React.useEffect(() => {
    if (autoClose && onClose) {
      const timer = setTimeout(onClose, autoClose);
      return () => clearTimeout(timer);
    }
  }, [autoClose, onClose]);

  return (
    <div className={`alert alert-${type} ${className}`}>
      <div className="alert-content">
        <span className="alert-icon">
          {type === 'error' && '✕'}
          {type === 'success' && '✓'}
          {type === 'warning' && '⚠'}
          {type === 'info' && 'ℹ'}
        </span>
        <span>{message}</span>
      </div>
      {dismissible && (
        <button
          className="alert-close"
          onClick={onClose}
          type="button"
          aria-label="Close alert"
        >
          ✕
        </button>
      )}
    </div>
  );
};

export default Alert;
