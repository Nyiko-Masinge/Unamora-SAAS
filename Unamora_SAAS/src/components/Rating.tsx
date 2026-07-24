import React from 'react';
import './Rating.css';

interface RatingProps {
  value?: number;
  max?: number;
  readOnly?: boolean;
  onChange?: (rating: number) => void;
  size?: 'small' | 'medium' | 'large';
  showLabel?: boolean;
  className?: string;
}

const Rating: React.FC<RatingProps> = ({
  value = 0,
  max = 5,
  readOnly = true,
  onChange,
  size = 'medium',
  showLabel = true,
  className = '',
}) => {
  return (
    <div className={`rating rating-${size} ${className}`}>
      <div className="stars">
        {Array.from({ length: max }).map((_, index) => (
          <button
            key={index}
            className={`star ${index < value ? 'filled' : ''}`}
            onClick={() => !readOnly && onChange?.(index + 1)}
            disabled={readOnly}
            type="button"
            aria-label={`Rate ${index + 1}`}
          >
            ★
          </button>
        ))}
      </div>
      {showLabel && <span className="rating-label">{value.toFixed(1)}</span>}
    </div>
  );
};

export default Rating;
