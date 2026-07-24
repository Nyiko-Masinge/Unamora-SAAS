import React from 'react';
import './Textarea.css';

interface TextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  label?: string;
  error?: string;
  helperText?: string;
  fullWidth?: boolean;
  characterLimit?: number;
}

const Textarea: React.FC<TextareaProps> = ({
  label,
  error,
  helperText,
  fullWidth = false,
  characterLimit,
  value = '',
  className = '',
  id,
  onChange,
  ...props
}) => {
  const inputId = id || `textarea-${Math.random()}`;
  const charCount = String(value).length;
  const isOverLimit = characterLimit && charCount > characterLimit;

  return (
    <div className={`textarea-wrapper ${fullWidth ? 'full-width' : ''} ${error ? 'error' : ''}`}>
      {label && <label htmlFor={inputId}>{label}</label>}
      <textarea
        id={inputId}
        className={`textarea ${className}`}
        value={value}
        onChange={onChange}
        {...props}
      />
      <div className="textarea-footer">
        {helperText && !error && <span className="helper-text">{helperText}</span>}
        {error && <span className="error-text">{error}</span>}
        {characterLimit && (
          <span className={`char-count ${isOverLimit ? 'over-limit' : ''}`}>
            {charCount}/{characterLimit}
          </span>
        )}
      </div>
    </div>
  );
};

export default Textarea;
