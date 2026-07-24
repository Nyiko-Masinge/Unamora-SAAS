import React from 'react';
import './Input.css';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string;
  helperText?: string;
  fullWidth?: boolean;
}

const Input: React.FC<InputProps> = ({
  label,
  error,
  helperText,
  fullWidth = false,
  className = '',
  id,
  ...props
}) => {
  const inputId = id || `input-${Math.random()}`;
  
  return (
    <div className={`input-wrapper ${fullWidth ? 'full-width' : ''} ${error ? 'error' : ''}`}>
      {label && <label htmlFor={inputId}>{label}</label>}
      <input id={inputId} className={`input ${className}`} {...props} />
      {error && <span className="error-text">{error}</span>}
      {helperText && !error && <span className="helper-text">{helperText}</span>}
    </div>
  );
};

export default Input;
