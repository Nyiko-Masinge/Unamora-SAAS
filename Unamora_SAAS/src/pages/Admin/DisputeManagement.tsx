import React, { useEffect, useState } from 'react';
import './DisputeManagement.css';

interface Dispute {
  id: string;
  title: string;
  claimantName: string;
  respondentName: string;
  category: string;
  status: string;
  claimedAmount: number;
  openedAt: string;
  closedAt?: string;
}

export const DisputeManagement: React.FC = () => {
  const [disputes, setDisputes] = useState<Dispute[]>([]);
  const [filteredDisputes, setFilteredDisputes] = useState<Dispute[]>([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState('all');
  const [categoryFilter, setCategoryFilter] = useState('all');
  const [selectedDispute, setSelectedDispute] = useState<Dispute | null>(null);
  const [showDetails, setShowDetails] = useState(false);

  useEffect(() => {
    fetchDisputes();
  }, []);

  useEffect(() => {
    filterDisputes();
  }, [disputes, statusFilter, categoryFilter]);

  const fetchDisputes = async () => {
    try {
      const response = await fetch('/api/admin/disputes/filter', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          pageNumber: 1,
          pageSize: 50,
        }),
      });

      if (response.ok) {
        const data = await response.json();
        setDisputes(data);
      }
      setLoading(false);
    } catch (error) {
      console.error('Failed to fetch disputes:', error);
      setLoading(false);
    }
  };

  const filterDisputes = () => {
    let filtered = disputes;

    if (statusFilter !== 'all') {
      filtered = filtered.filter((d) => d.status.toLowerCase() === statusFilter.toLowerCase());
    }

    if (categoryFilter !== 'all') {
      filtered = filtered.filter((d) => d.category.toLowerCase() === categoryFilter.toLowerCase());
    }

    setFilteredDisputes(filtered);
  };

  const getStatusColor = (status: string): string => {
    switch (status.toLowerCase()) {
      case 'opened':
        return 'warning';
      case 'underreview':
        return 'info';
      case 'closed':
        return 'success';
      case 'appealed':
        return 'danger';
      default:
        return 'secondary';
    }
  };

  const handleResolveDispute = async (disputeId: string) => {
    const resolution = prompt('Enter resolution (0=Refund, 1=Payment, 2=Split, 3=Dismiss):');
    if (resolution !== null) {
      try {
        const response = await fetch('/api/admin/dispute/resolve', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            disputeId,
            resolution: parseInt(resolution),
            resolutionNotes: 'Resolved by admin',
          }),
        });

        if (response.ok) {
          alert('Dispute resolved successfully');
          fetchDisputes();
        }
      } catch (error) {
        console.error('Failed to resolve dispute:', error);
        alert('Failed to resolve dispute');
      }
    }
  };

  if (loading) return <div className="loading">Loading disputes...</div>;

  return (
    <div className="dispute-management">
      <div className="management-header">
        <h1>Dispute Resolution</h1>
        <p>Manage and resolve platform disputes</p>
      </div>

      <div className="filters">
        <div className="filter-group">
          <label htmlFor="status-filter">Status:</label>
          <select
            id="status-filter"
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
          >
            <option value="all">All Statuses</option>
            <option value="opened">Opened</option>
            <option value="underreview">Under Review</option>
            <option value="closed">Closed</option>
            <option value="appealed">Appealed</option>
          </select>
        </div>

        <div className="filter-group">
          <label htmlFor="category-filter">Category:</label>
          <select
            id="category-filter"
            value={categoryFilter}
            onChange={(e) => setCategoryFilter(e.target.value)}
          >
            <option value="all">All Categories</option>
            <option value="payment">Payment Issue</option>
            <option value="quality">Service Quality</option>
            <option value="cancellation">Cancellation</option>
            <option value="noshow">No Show</option>
            <option value="damage">Damage or Loss</option>
          </select>
        </div>

        <div className="filter-info">
          Showing {filteredDisputes.length} of {disputes.length} disputes
        </div>
      </div>

      <div className="disputes-grid">
        {filteredDisputes.length === 0 ? (
          <p className="no-data">No disputes found</p>
        ) : (
          filteredDisputes.map((dispute) => (
            <div
              key={dispute.id}
              className="dispute-card"
              onClick={() => {
                setSelectedDispute(dispute);
                setShowDetails(true);
              }}
            >
              <div className="dispute-header">
                <h3>{dispute.title}</h3>
                <span className={`status-badge ${getStatusColor(dispute.status)}`}>
                  {dispute.status}
                </span>
              </div>

              <div className="dispute-details">
                <p>
                  <strong>Claimant:</strong> {dispute.claimantName}
                </p>
                <p>
                  <strong>Respondent:</strong> {dispute.respondentName}
                </p>
                <p>
                  <strong>Amount:</strong> ${dispute.claimedAmount.toFixed(2)}
                </p>
                <p>
                  <strong>Category:</strong> {dispute.category}
                </p>
                <p>
                  <strong>Opened:</strong> {new Date(dispute.openedAt).toLocaleDateString()}
                </p>
              </div>

              <div className="dispute-actions">
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    handleResolveDispute(dispute.id);
                  }}
                  className="action-btn resolve"
                >
                  Resolve
                </button>
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    setSelectedDispute(dispute);
                    setShowDetails(true);
                  }}
                  className="action-btn details"
                >
                  View Details
                </button>
              </div>
            </div>
          ))
        )}
      </div>

      {showDetails && selectedDispute && (
        <div className="dispute-modal">
          <div className="modal-content">
            <button className="close-btn" onClick={() => setShowDetails(false)}>
              ✕
            </button>
            <h2>{selectedDispute.title}</h2>
            <div className="modal-body">
              <div className="modal-section">
                <h3>Dispute Information</h3>
                <p>
                  <strong>Status:</strong> {selectedDispute.status}
                </p>
                <p>
                  <strong>Category:</strong> {selectedDispute.category}
                </p>
                <p>
                  <strong>Claimed Amount:</strong> ${selectedDispute.claimedAmount.toFixed(2)}
                </p>
                <p>
                  <strong>Opened:</strong> {new Date(selectedDispute.openedAt).toLocaleString()}
                </p>
                {selectedDispute.closedAt && (
                  <p>
                    <strong>Closed:</strong> {new Date(selectedDispute.closedAt).toLocaleString()}
                  </p>
                )}
              </div>

              <div className="modal-section">
                <h3>Parties Involved</h3>
                <p>
                  <strong>Claimant:</strong> {selectedDispute.claimantName}
                </p>
                <p>
                  <strong>Respondent:</strong> {selectedDispute.respondentName}
                </p>
              </div>

              <div className="modal-actions">
                <button
                  onClick={() => {
                    handleResolveDispute(selectedDispute.id);
                    setShowDetails(false);
                  }}
                  className="btn btn-primary"
                >
                  Resolve Dispute
                </button>
                <button
                  onClick={() => setShowDetails(false)}
                  className="btn btn-secondary"
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
