import React, { useState, useEffect } from 'react';
import './PaymentPage.css';

interface Subscription {
  id: string;
  tier: number;
  tierName: string;
  monthlyRate: number;
  annualRate?: number;
  features: string[];
}

export const PaymentPage: React.FC = () => {
  const [subscriptions, setSubscriptions] = useState<Subscription[]>([]);
  const [currentSubscription, setCurrentSubscription] = useState<Subscription | null>(null);
  const [billingAnnually, setBillingAnnually] = useState(false);
  const [loading, setLoading] = useState(true);
  const [paymentHistory, setPaymentHistory] = useState<any[]>([]);

  useEffect(() => {
    fetchSubscriptions();
    fetchCurrentSubscription();
    fetchPaymentHistory();
  }, []);

  const fetchSubscriptions = async () => {
    try {
      const response = await fetch('/api/payment/subscriptions/available');
      const data = await response.json();
      setSubscriptions(data);
    } catch (error) {
      console.error('Failed to fetch subscriptions:', error);
    }
  };

  const fetchCurrentSubscription = async () => {
    try {
      const response = await fetch('/api/payment/subscription');
      if (response.ok) {
        const data = await response.json();
        setCurrentSubscription(data);
      }
    } catch (error) {
      console.error('Failed to fetch current subscription:', error);
    }
  };

  const fetchPaymentHistory = async () => {
    try {
      const response = await fetch('/api/payment/history');
      const data = await response.json();
      setPaymentHistory(data);
      setLoading(false);
    } catch (error) {
      console.error('Failed to fetch payment history:', error);
      setLoading(false);
    }
  };

  const handleUpgradeSubscription = async (tier: number) => {
    try {
      const response = await fetch('/api/payment/subscription', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          tier,
          billingAnnually,
        }),
      });

      if (response.ok) {
        alert('Subscription updated successfully!');
        fetchCurrentSubscription();
        fetchPaymentHistory();
      }
    } catch (error) {
      console.error('Failed to upgrade subscription:', error);
      alert('Failed to update subscription');
    }
  };

  if (loading) return <div className="loading">Loading...</div>;

  return (
    <div className="payment-page">
      <div className="current-subscription">
        <h2>Your Subscription</h2>
        {currentSubscription ? (
          <div className="subscription-card">
            <h3>{currentSubscription.tierName}</h3>
            <p className="price">
              ${billingAnnually ? currentSubscription.annualRate : currentSubscription.monthlyRate}
              <span>/{billingAnnually ? 'year' : 'month'}</span>
            </p>
            <ul className="features">
              {currentSubscription.features?.map((feature, index) => (
                <li key={index}>✓ {feature}</li>
              ))}
            </ul>
          </div>
        ) : (
          <p>You don't have an active subscription</p>
        )}
      </div>

      <div className="subscriptions-section">
        <h2>Upgrade Your Plan</h2>
        <div className="billing-toggle">
          <label>
            <input
              type="checkbox"
              checked={billingAnnually}
              onChange={(e) => setBillingAnnually(e.target.checked)}
            />
            Bill Annually (Save 20%)
          </label>
        </div>

        <div className="subscriptions-grid">
          {subscriptions.map((sub) => (
            <div
              key={sub.id}
              className={`subscription-option ${currentSubscription?.id === sub.id ? 'current' : ''}`}
            >
              <h3>{sub.tierName}</h3>
              <p className="price">
                ${billingAnnually ? sub.annualRate : sub.monthlyRate}
                <span>/{billingAnnually ? 'year' : 'month'}</span>
              </p>
              <ul className="features">
                {sub.features?.map((feature, index) => (
                  <li key={index}>✓ {feature}</li>
                ))}
              </ul>
              <button
                onClick={() => handleUpgradeSubscription(sub.tier)}
                className={`upgrade-btn ${currentSubscription?.id === sub.id ? 'current' : ''}`}
              >
                {currentSubscription?.id === sub.id ? 'Current Plan' : 'Upgrade'}
              </button>
            </div>
          ))}
        </div>
      </div>

      <div className="payment-history-section">
        <h2>Payment History</h2>
        <div className="history-table">
          <table>
            <thead>
              <tr>
                <th>Date</th>
                <th>Amount</th>
                <th>Type</th>
                <th>Status</th>
                <th>Receipt</th>
              </tr>
            </thead>
            <tbody>
              {paymentHistory.map((payment) => (
                <tr key={payment.paymentId}>
                  <td>{new Date(payment.processedAt).toLocaleDateString()}</td>
                  <td>${payment.amount.toFixed(2)}</td>
                  <td>{payment.bookingDescription}</td>
                  <td>
                    <span className={`status ${payment.status.toLowerCase()}`}>
                      {payment.status}
                    </span>
                  </td>
                  <td>
                    <a href={`/api/payment/${payment.paymentId}/receipt`} className="receipt-link">
                      Download
                    </a>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};
