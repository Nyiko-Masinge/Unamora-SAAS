import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import Button from '../../components/Button';
import Badge from '../../components/Badge';
import Rating from '../../components/Rating';
import Avatar from '../../components/Avatar';
import Card from '../../components/Card';
import './ProfileView.css';

const ProfileView: React.FC = () => {
  const { userId } = useParams();
  const { user } = useAuth();
  const navigate = useNavigate();
  const [isSaved, setIsSaved] = useState(false);

  // Mock professional data - replace with API call
  const professional = {
    id: userId,
    firstName: 'John',
    lastName: 'Doe',
    businessName: 'John\'s Express Plumbing',
    headline: 'Master Plumber - 24/7 Leak & Drain Expert',
    photo: '',
    bio: 'With over 12 years of experience in residential and commercial plumbing. Fully insured and SA certified.',
    yearsOfExperience: 12,
    rating: 4.85,
    reviewCount: 28,
    completedJobs: 145,
    responseTime: 15,
    city: 'Johannesburg',
    serviceRadius: 30,
    hourlyRateMin: 350,
    hourlyRateMax: 550,
    verification: {
      status: 'Approved',
      policeClearance: true,
      insuranceVerified: true,
      referencesVerified: true,
    },
    skills: [
      { name: 'Leak Repair', level: 'Expert' },
      { name: 'Drain Unblocking', level: 'Advanced' },
      { name: 'Pipe Installation', level: 'Expert' },
    ],
    portfolio: [
      { title: 'Kitchen Renovation', image: '', date: 'March 2024' },
      { title: 'Bathroom Redesign', image: '', date: 'February 2024' },
      { title: 'Emergency Leak Fix', image: '', date: 'January 2024' },
    ],
    recentReviews: [
      {
        id: 1,
        author: 'Sarah M.',
        rating: 5,
        comment: 'Excellent work! Very professional and punctual.',
        date: '2 weeks ago',
      },
      {
        id: 2,
        author: 'James K.',
        rating: 5,
        comment: 'Quick response and great quality. Highly recommended!',
        date: '1 month ago',
      },
    ],
  };

  return (
    <div className="profile-view">
      <div className="profile-header">
        <div className="header-background" />
        <div className="profile-hero">
          <Avatar
            initials={professional.firstName[0] + professional.lastName[0]}
            size="xlarge"
          />
          <div className="profile-info">
            <h1>{professional.businessName}</h1>
            <p className="headline">{professional.headline}</p>
            <div className="location-rating">
              <span>{professional.city} · {professional.serviceRadius}km radius</span>
              <div className="rating-display">
                <Rating value={professional.rating} readOnly showLabel />
                <span className="review-count">({professional.reviewCount} reviews)</span>
              </div>
            </div>
          </div>
          <div className="profile-actions">
            <Button variant="primary" size="medium" onClick={() => navigate('/search')}>
              Book Now
            </Button>
            <Button
              variant="secondary"
              size="medium"
              onClick={() => setIsSaved(!isSaved)}
            >
              {isSaved ? '✓ Saved' : 'Save Profile'}
            </Button>
          </div>
        </div>
      </div>

      <div className="profile-content">
        <div className="profile-main">
          {/* About */}
          <Card variant="elevated" className="section">
            <h2>About</h2>
            <p>{professional.bio}</p>
            <div className="stats-grid">
              <div className="stat">
                <strong>{professional.yearsOfExperience}</strong>
                <span>Years Experience</span>
              </div>
              <div className="stat">
                <strong>{professional.completedJobs}</strong>
                <span>Jobs Completed</span>
              </div>
              <div className="stat">
                <strong>{professional.responseTime}m</strong>
                <span>Avg Response</span>
              </div>
            </div>
          </Card>

          {/* Hourly Rates */}
          <Card variant="elevated" className="section">
            <h2>Pricing</h2>
            <div className="pricing">
              <div className="price-range">
                <span className="label">Hourly Rate</span>
                <span className="price">
                  R{professional.hourlyRateMin} - R{professional.hourlyRateMax}/hr
                </span>
              </div>
            </div>
          </Card>

          {/* Skills */}
          <Card variant="elevated" className="section">
            <h2>Skills & Expertise</h2>
            <div className="skills-grid">
              {professional.skills.map((skill) => (
                <div key={skill.name} className="skill-item">
                  <strong>{skill.name}</strong>
                  <Badge label={skill.level} variant="info" />
                </div>
              ))}
            </div>
          </Card>

          {/* Portfolio */}
          <Card variant="elevated" className="section">
            <h2>Recent Work</h2>
            <div className="portfolio-grid">
              {professional.portfolio.map((project) => (
                <div key={project.title} className="portfolio-item">
                  <div className="portfolio-image" />
                  <h4>{project.title}</h4>
                  <p>{project.date}</p>
                </div>
              ))}
            </div>
          </Card>

          {/* Reviews */}
          <Card variant="elevated" className="section">
            <h2>Recent Reviews</h2>
            <div className="reviews-list">
              {professional.recentReviews.map((review) => (
                <div key={review.id} className="review-item">
                  <div className="review-header">
                    <strong>{review.author}</strong>
                    <Rating value={review.rating} readOnly={true} size="small" showLabel={false} />
                  </div>
                  <p className="review-text">{review.comment}</p>
                  <span className="review-date">{review.date}</span>
                </div>
              ))}
            </div>
          </Card>
        </div>

        <div className="profile-sidebar">
          {/* Verification */}
          <Card variant="elevated" className="sidebar-card">
            <h3>Verification</h3>
            <div className="verification-items">
              <div className="verification-item">
                <span className="check">✓</span>
                <div>
                  <strong>Identity Verified</strong>
                  <p>Approved</p>
                </div>
              </div>
              <div className="verification-item">
                <span className="check">✓</span>
                <div>
                  <strong>Police Clearance</strong>
                  <p>Verified</p>
                </div>
              </div>
              <div className="verification-item">
                <span className="check">✓</span>
                <div>
                  <strong>Insurance</strong>
                  <p>Verified</p>
                </div>
              </div>
            </div>
          </Card>

          {/* Contact */}
          <Card variant="elevated" className="sidebar-card">
            <h3>Contact</h3>
            <Button fullWidth variant="primary" size="medium">
              Send Message
            </Button>
            <p style={{ fontSize: '0.875rem', color: '#666', marginTop: '1rem', textAlign: 'center' }}>
              Typical response time: {professional.responseTime} minutes
            </p>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default ProfileView;
