import React, { useEffect, useState } from 'react';
import './AdminDashboard.css';

interface DashboardStats {
  totalUsers: number;
  activeUsers: number;
  totalBookings: number;
  completedBookings: number;
  totalRevenue: number;
  monthlyRevenue: number;
  openDisputes: number;
  pendingVerifications: number;
  averageRating: number;
  totalPaymentsProcessed: number;
}

interface RecentActivity {
  id: string;
  type: string;
  description: string;
  userName: string;
  timestamp: string;
}

export const AdminDashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [activities, setActivities] = useState<RecentActivity[]>([]);
  const [loading, setLoading] = useState(true);
  const [chartData, setChartData] = useState<any>(null);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      const [statsRes, activitiesRes, chartRes] = await Promise.all([
        fetch('/api/admin/dashboard/statistics'),
        fetch('/api/admin/dashboard/recent-activities'),
        fetch('/api/admin/dashboard/revenue?period=monthly'),
      ]);

      if (statsRes.ok) {
        const statsData = await statsRes.json();
        setStats(statsData);
      }

      if (activitiesRes.ok) {
        const activitiesData = await activitiesRes.json();
        setActivities(activitiesData);
      }

      if (chartRes.ok) {
        const chartDataValue = await chartRes.json();
        setChartData(chartDataValue);
      }

      setLoading(false);
    } catch (error) {
      console.error('Failed to fetch dashboard data:', error);
      setLoading(false);
    }
  };

  if (loading) return <div className="loading">Loading dashboard...</div>;

  return (
    <div className="admin-dashboard">
      <div className="dashboard-header">
        <h1>Admin Dashboard</h1>
        <p>Welcome back! Here's your platform overview.</p>
      </div>

      {stats && (
        <div className="stats-grid">
          <div className="stat-card">
            <div className="stat-icon">👥</div>
            <div className="stat-content">
              <h3>{stats.totalUsers.toLocaleString()}</h3>
              <p>Total Users</p>
              <span className="trend positive">+12% this month</span>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon">📅</div>
            <div className="stat-content">
              <h3>{stats.totalBookings.toLocaleString()}</h3>
              <p>Total Bookings</p>
              <span className="trend positive">+8% this month</span>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon">💰</div>
            <div className="stat-content">
              <h3>${(stats.totalRevenue / 1000).toFixed(1)}K</h3>
              <p>Total Revenue</p>
              <span className="trend positive">+15% this month</span>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon">⚠️</div>
            <div className="stat-content">
              <h3>{stats.openDisputes}</h3>
              <p>Open Disputes</p>
              <span className="trend negative">Needs attention</span>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon">✓</div>
            <div className="stat-content">
              <h3>{stats.pendingVerifications}</h3>
              <p>Pending Verifications</p>
              <span className="trend warning">Action required</span>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon">⭐</div>
            <div className="stat-content">
              <h3>{stats.averageRating.toFixed(1)}</h3>
              <p>Platform Rating</p>
              <span className="trend positive">Excellent</span>
            </div>
          </div>
        </div>
      )}

      <div className="dashboard-content">
        <div className="chart-section">
          <div className="chart-card">
            <h2>Revenue Trend</h2>
            {chartData && (
              <div className="chart-placeholder">
                <p>Revenue chart would display here</p>
                <p>Monthly revenue data visualization</p>
              </div>
            )}
          </div>

          <div className="chart-card">
            <h2>Key Metrics</h2>
            <div className="metrics-list">
              {stats && (
                <>
                  <div className="metric">
                    <span className="label">Active Users</span>
                    <span className="value">{stats.activeUsers.toLocaleString()}</span>
                  </div>
                  <div className="metric">
                    <span className="label">Completed Bookings</span>
                    <span className="value">{stats.completedBookings.toLocaleString()}</span>
                  </div>
                  <div className="metric">
                    <span className="label">Monthly Revenue</span>
                    <span className="value">${stats.monthlyRevenue.toLocaleString()}</span>
                  </div>
                  <div className="metric">
                    <span className="label">Payments Processed</span>
                    <span className="value">{stats.totalPaymentsProcessed.toLocaleString()}</span>
                  </div>
                </>
              )}
            </div>
          </div>
        </div>

        <div className="activities-section">
          <h2>Recent Activities</h2>
          <div className="activities-list">
            {activities.length === 0 ? (
              <p className="no-data">No recent activities</p>
            ) : (
              activities.map((activity) => (
                <div key={activity.id} className="activity-item">
                  <div className="activity-icon">
                    {activity.type === 'booking' && '📅'}
                    {activity.type === 'payment' && '💳'}
                    {activity.type === 'verification' && '✓'}
                    {activity.type === 'dispute' && '⚠️'}
                    {activity.type === 'user' && '👤'}
                  </div>
                  <div className="activity-content">
                    <p className="activity-description">{activity.description}</p>
                    <p className="activity-user">{activity.userName}</p>
                  </div>
                  <div className="activity-time">
                    {new Date(activity.timestamp).toLocaleTimeString()}
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>

      <div className="quick-actions">
        <h2>Quick Actions</h2>
        <div className="actions-grid">
          <a href="/admin/verifications" className="action-btn">
            📋 Review Verifications
          </a>
          <a href="/admin/disputes" className="action-btn">
            ⚠️ Review Disputes
          </a>
          <a href="/admin/users" className="action-btn">
            👥 Manage Users
          </a>
          <a href="/admin/payments" className="action-btn">
            💳 View Payments
          </a>
          <a href="/admin/reports" className="action-btn">
            📊 Generate Reports
          </a>
          <a href="/admin/bookings" className="action-btn">
            📅 View Bookings
          </a>
        </div>
      </div>
    </div>
  );
};
