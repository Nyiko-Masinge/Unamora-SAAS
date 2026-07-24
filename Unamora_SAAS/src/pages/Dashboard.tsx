import React, { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { api } from '../services/api';

type Role = 'Client' | 'Tradesperson' | 'Admin';

interface DashboardData {
  eyebrow: string;
  title: string;
  description: string;
  metrics: [string, string][];
  actions: string[];
}

const dashboards: Record<string, DashboardData> = {
  Client: {
    eyebrow: 'Client workspace',
    title: 'Your home projects, organised.',
    description: 'Track upcoming work, message verified pros, and keep every receipt in one place.',
    metrics: [
      ['Next booking', 'Thu, 09:00'],
      ['Unread messages', '2'],
      ['Saved professionals', '8']
    ],
    actions: ['Find a tradesperson', 'View my bookings', 'Message inbox']
  },
  Tradesperson: {
    eyebrow: 'Tradesperson workspace',
    title: 'Keep your business moving.',
    description: 'Manage leads, availability, bookings, and the trust signals clients look for.',
    metrics: [
      ['New matched leads', '6'],
      ['Upcoming bookings', '4'],
      ['Profile completion', '92%']
    ],
    actions: ['View matched jobs', 'Manage availability', 'Complete verification']
  },
  Admin: {
    eyebrow: 'Operations workspace',
    title: 'A clearer view of marketplace health.',
    description: 'Review verification, disputes, risk signals, and operational queues with complete audit trails.',
    metrics: [
      ['Verification queue', '14'],
      ['Open disputes', '3'],
      ['Risk reviews', '7']
    ],
    actions: ['Open verification queue', 'Review disputes', 'View risk cases']
  },
};

const Dashboard: React.FC = () => {
  const { user } = useAuth();
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (user) {
      const roleData = dashboards[user.role] || dashboards.Client;
      setData(roleData);
      setLoading(false);
    }
  }, [user]);

  if (loading || !data || !user) {
    return <div className="loading">Loading your dashboard...</div>;
  }

  return (
    <section className="dashboard">
      <aside>
        <a className="brand" href="/">unamora<span>.</span></a>
        <p>Workspace</p>
        <div className="role-info">
          <p><strong>{user.firstName} {user.lastName}</strong></p>
          <p className="role-badge">{user.role}</p>
        </div>
        <nav className="side-nav">
          <a href="/dashboard" className="active">Dashboard</a>
          <a href="/search">Search</a>
          <a href="/chat">Messages</a>
          <a href="/payments">Payments</a>
          <a href="/reviews">Reviews</a>
          <a href="/settings">Settings</a>
        </nav>
        <div className="side-links">
          <a href="#logout">Logout</a>
        </div>
      </aside>
      <div className="dashboard-main">
        <p className="eyebrow">{data.eyebrow}</p>
        <h1>{data.title}</h1>
        <p className="dashboard-copy">{data.description}</p>

        <div className="metric-grid">
          {data.metrics.map(([label, value]) => (
            <article className="metric" key={label}>
              <span>{label}</span>
              <strong>{value}</strong>
            </article>
          ))}
        </div>

        <section className="work-panel">
          <div>
            <h2>What would you like to do?</h2>
            <p>Choose an action to get where you need to go.</p>
          </div>
          <div className="action-list">
            {data.actions.map((action) => (
              <button key={action}>
                {action}
                <span>→</span>
              </button>
            ))}
          </div>
        </section>
      </div>
    </section>
  );
};

export default Dashboard;
