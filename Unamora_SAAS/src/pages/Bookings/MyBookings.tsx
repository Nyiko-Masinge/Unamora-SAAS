import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '../../components/Button';
import Card from '../../components/Card';
import Avatar from '../../components/Avatar';
import Badge from '../../components/Badge';
import './Bookings.css';

const MyBookings: React.FC = () => {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<'active' | 'completed'>('active');

  const bookings = {
    active: [
      {
        id: 1,
        professional: 'John Doe',
        service: 'Leak Repair',
        date: 'Thu, 24 July 2026',
        time: '09:00 AM',
        status: 'scheduled',
        location: 'Johannesburg',
        amount: 'R 450',
      },
      {
        id: 2,
        professional: 'Sparky Jones',
        service: 'DB Board Installation',
        date: 'Sat, 26 July 2026',
        time: '02:00 PM',
        status: 'confirmed',
        location: 'Johannesburg',
        amount: 'R 1,200',
      },
    ],
    completed: [
      {
        id: 3,
        professional: 'John Doe',
        service: 'Pipe Installation',
        date: 'Mon, 15 July 2026',
        status: 'completed',
        location: 'Johannesburg',
        amount: 'R 650',
        rating: 5,
      },
    ],
  };

  return (
    <div className="bookings-page">
      <div className="page-header">
        <h1>My Bookings</h1>
        <Button variant="primary" size="medium" onClick={() => navigate('/post-job')}>
          + New Job
        </Button>
      </div>

      <div className="tabs">
        <button
          className={`tab ${activeTab === 'active' ? 'active' : ''}`}
          onClick={() => setActiveTab('active')}
        >
          Active Bookings ({bookings.active.length})
        </button>
        <button
          className={`tab ${activeTab === 'completed' ? 'active' : ''}`}
          onClick={() => setActiveTab('completed')}
        >
          Completed ({bookings.completed.length})
        </button>
      </div>

      <div className="bookings-list">
        {activeTab === 'active' ? (
          bookings.active.length > 0 ? (
            bookings.active.map((booking) => (
              <Card key={booking.id} variant="elevated" className="booking-card">
                <div className="booking-header">
                  <div className="booking-info">
                    <h3>{booking.service}</h3>
                    <div className="booking-details">
                      <span className="date">{booking.date} at {booking.time}</span>
                      <span className="location">📍 {booking.location}</span>
                    </div>
                  </div>
                  <Badge label={booking.status} variant="info" />
                </div>
                <div className="booking-pro">
                  <Avatar initials={booking.professional.charAt(0) + booking.professional.charAt(booking.professional.indexOf(' ') + 1)} size="medium" />
                  <div>
                    <p className="pro-name">{booking.professional}</p>
                    <p className="pro-title">Verified Professional</p>
                  </div>
                </div>
                <div className="booking-footer">
                  <span className="amount">{booking.amount}</span>
                  <div className="actions">
                    <Button variant="secondary" size="small">Message</Button>
                    <Button variant="primary" size="small">Track</Button>
                  </div>
                </div>
              </Card>
            ))
          ) : (
            <Card variant="elevated" className="empty-state">
              <p>No active bookings yet</p>
              <Button variant="primary" size="medium" onClick={() => navigate('/post-job')}>
                Post Your First Job
              </Button>
            </Card>
          )
        ) : (
          bookings.completed.map((booking) => (
            <Card key={booking.id} variant="elevated" className="booking-card">
              <div className="booking-header">
                <div className="booking-info">
                  <h3>{booking.service}</h3>
                  <div className="booking-details">
                    <span className="date">{booking.date}</span>
                    <span className="location">📍 {booking.location}</span>
                  </div>
                </div>
                <Badge label="Completed" variant="success" />
              </div>
              <div className="booking-pro">
                <Avatar initials={booking.professional.charAt(0) + booking.professional.charAt(booking.professional.indexOf(' ') + 1)} size="medium" />
                <div>
                  <p className="pro-name">{booking.professional}</p>
                  <p className="pro-title">Rating: ⭐ {booking.rating}/5</p>
                </div>
              </div>
              <div className="booking-footer">
                <span className="amount">{booking.amount}</span>
                <div className="actions">
                  <Button variant="secondary" size="small">View Invoice</Button>
                  <Button variant="primary" size="small">Leave Review</Button>
                </div>
              </div>
            </Card>
          ))
        )}
      </div>
    </div>
  );
};

export default MyBookings;
