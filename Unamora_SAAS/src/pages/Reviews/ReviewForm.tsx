import React, { useState } from 'react';
import './ReviewForm.css';

interface ReviewRating {
  category: number;
  score: number;
}

export const ReviewForm: React.FC<{ bookingId: string; revieweeId: string; onSubmit: () => void }> = ({
  bookingId,
  revieweeId,
  onSubmit,
}) => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [ratings, setRatings] = useState<ReviewRating[]>([
    { category: 0, score: 5 }, // Quality
    { category: 1, score: 5 }, // Communication
    { category: 2, score: 5 }, // Timeliness
    { category: 3, score: 5 }, // Professionalism
    { category: 4, score: 5 }, // Value
  ]);
  const [attachments, setAttachments] = useState<File[]>([]);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const categoryNames = ['Quality', 'Communication', 'Timeliness', 'Professionalism', 'Value'];

  const handleRatingChange = (categoryIndex: number, score: number) => {
    const newRatings = [...ratings];
    newRatings[categoryIndex].score = score;
    setRatings(newRatings);
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) {
      setAttachments([...attachments, ...Array.from(e.target.files)]);
    }
  };

  const removeAttachment = (index: number) => {
    setAttachments(attachments.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);

    try {
      // Upload attachments first
      let attachmentUrls: string[] = [];
      for (const file of attachments) {
        const formData = new FormData();
        formData.append('file', file);
        const uploadResponse = await fetch('/api/upload', {
          method: 'POST',
          body: formData,
        });
        const { url } = await uploadResponse.json();
        attachmentUrls.push(url);
      }

      // Submit review
      const response = await fetch('/api/review', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          bookingId,
          revieweeId,
          title,
          description,
          ratings,
          attachmentUrls,
        }),
      });

      if (response.ok) {
        alert('Review submitted successfully!');
        onSubmit();
      }
    } catch (error) {
      console.error('Failed to submit review:', error);
      alert('Failed to submit review');
    } finally {
      setIsSubmitting(false);
    }
  };

  const averageRating =
    ratings.reduce((sum, r) => sum + r.score, 0) / ratings.length;

  return (
    <form className="review-form" onSubmit={handleSubmit}>
      <div className="overall-rating">
        <label>Overall Rating</label>
        <div className="star-rating">
          {[1, 2, 3, 4, 5].map((star) => (
            <span
              key={star}
              className={`star ${star <= Math.round(averageRating) ? 'filled' : ''}`}
            >
              ★
            </span>
          ))}
          <span className="rating-value">{averageRating.toFixed(1)}/5</span>
        </div>
      </div>

      <div className="form-group">
        <label htmlFor="title">Review Title</label>
        <input
          id="title"
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="e.g., Excellent service, very professional"
          required
        />
      </div>

      <div className="form-group">
        <label htmlFor="description">Description</label>
        <textarea
          id="description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          placeholder="Share your experience with this professional..."
          rows={5}
          required
        />
      </div>

      <div className="ratings-section">
        <label>Rate by Category</label>
        {ratings.map((rating, index) => (
          <div key={rating.category} className="rating-category">
            <label>{categoryNames[index]}</label>
            <div className="rating-stars">
              {[1, 2, 3, 4, 5].map((star) => (
                <button
                  key={star}
                  type="button"
                  className={`star-btn ${star <= rating.score ? 'active' : ''}`}
                  onClick={() => handleRatingChange(index, star)}
                >
                  ★
                </button>
              ))}
              <span className="score">{rating.score}/5</span>
            </div>
          </div>
        ))}
      </div>

      <div className="form-group">
        <label>Attach Photos/Videos</label>
        <input
          type="file"
          multiple
          accept="image/*,video/*"
          onChange={handleFileChange}
        />
        {attachments.length > 0 && (
          <div className="attachments-list">
            {attachments.map((file, index) => (
              <div key={index} className="attachment-item">
                <span>{file.name}</span>
                <button type="button" onClick={() => removeAttachment(index)}>
                  ✕
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      <button type="submit" className="submit-btn" disabled={isSubmitting}>
        {isSubmitting ? 'Submitting...' : 'Submit Review'}
      </button>
    </form>
  );
};
