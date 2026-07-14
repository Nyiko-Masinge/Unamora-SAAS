import React, { useEffect, useState } from 'react';
import './ReviewDisplay.css';

interface Review {
  id: string;
  title: string;
  description: string;
  overallRating: number;
  reviewerName: string;
  reviewerProfilePicture?: string;
  createdAt: string;
  ratings: { category: string; score: number }[];
  attachments: { id: string; fileUrl: string; fileType: string }[];
  replies: { id: string; content: string; responderName: string; createdAt: string }[];
}

export const ReviewDisplay: React.FC<{ userId: string }> = ({ userId }) => {
  const [reviews, setReviews] = useState<Review[]>([]);
  const [statistics, setStatistics] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchReviews();
    fetchStatistics();
  }, [userId]);

  const fetchReviews = async () => {
    try {
      const response = await fetch(`/api/review/user/${userId}`);
      const data = await response.json();
      setReviews(data);
    } catch (error) {
      console.error('Failed to fetch reviews:', error);
    }
  };

  const fetchStatistics = async () => {
    try {
      const response = await fetch(`/api/review/user/${userId}/statistics`);
      const data = await response.json();
      setStatistics(data);
      setLoading(false);
    } catch (error) {
      console.error('Failed to fetch statistics:', error);
      setLoading(false);
    }
  };

  if (loading) return <div className="loading">Loading reviews...</div>;

  return (
    <div className="review-display">
      {statistics && (
        <div className="review-statistics">
          <div className="stat-card">
            <div className="stat-value">{statistics.averageRating.toFixed(1)}</div>
            <div className="stat-label">Average Rating</div>
            <div className="star-rating">
              {[1, 2, 3, 4, 5].map((star) => (
                <span
                  key={star}
                  className={`star ${star <= Math.round(statistics.averageRating) ? 'filled' : ''}`}
                >
                  ★
                </span>
              ))}
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-value">{statistics.totalReviews}</div>
            <div className="stat-label">Total Reviews</div>
          </div>

          <div className="stat-card">
            <div className="rating-breakdown">
              {[5, 4, 3, 2, 1].map((rating) => (
                <div key={rating} className="breakdown-row">
                  <span className="rating-label">{rating}★</span>
                  <div className="progress-bar">
                    <div
                      className="progress-fill"
                      style={{
                        width: `${
                          (statistics[`ratingBreakdown_${rating}Stars`] /
                            statistics.totalReviews) *
                          100
                        }%`,
                      }}
                    ></div>
                  </div>
                  <span className="count">
                    {statistics[`ratingBreakdown_${rating}Stars`]}
                  </span>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}

      <div className="reviews-list">
        <h3>Reviews ({reviews.length})</h3>
        {reviews.length === 0 ? (
          <p className="no-reviews">No reviews yet</p>
        ) : (
          reviews.map((review) => (
            <div key={review.id} className="review-card">
              <div className="review-header">
                <div className="reviewer-info">
                  {review.reviewerProfilePicture && (
                    <img
                      src={review.reviewerProfilePicture}
                      alt={review.reviewerName}
                      className="reviewer-avatar"
                    />
                  )}
                  <div>
                    <h4>{review.reviewerName}</h4>
                    <p className="review-date">
                      {new Date(review.createdAt).toLocaleDateString()}
                    </p>
                  </div>
                </div>
                <div className="star-rating">
                  {[1, 2, 3, 4, 5].map((star) => (
                    <span
                      key={star}
                      className={`star ${star <= review.overallRating ? 'filled' : ''}`}
                    >
                      ★
                    </span>
                  ))}
                </div>
              </div>

              <h4 className="review-title">{review.title}</h4>
              <p className="review-description">{review.description}</p>

              {review.ratings && review.ratings.length > 0 && (
                <div className="review-ratings">
                  {review.ratings.map((rating) => (
                    <div key={rating.category} className="rating-item">
                      <span className="category">{rating.category}</span>
                      <span className="score">
                        {[...Array(rating.score)].map((_, i) => (
                          <span key={i} className="star filled">
                            ★
                          </span>
                        ))}
                      </span>
                    </div>
                  ))}
                </div>
              )}

              {review.attachments && review.attachments.length > 0 && (
                <div className="review-attachments">
                  {review.attachments.map((att) => (
                    <a
                      key={att.id}
                      href={att.fileUrl}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="attachment-thumbnail"
                    >
                      📎 Photo/Video
                    </a>
                  ))}
                </div>
              )}

              {review.replies && review.replies.length > 0 && (
                <div className="review-replies">
                  {review.replies.map((reply) => (
                    <div key={reply.id} className="reply">
                      <strong>{reply.responderName}</strong>
                      <p>{reply.content}</p>
                      <small>{new Date(reply.createdAt).toLocaleDateString()}</small>
                    </div>
                  ))}
                </div>
              )}
            </div>
          ))
        )}
      </div>
    </div>
  );
};
