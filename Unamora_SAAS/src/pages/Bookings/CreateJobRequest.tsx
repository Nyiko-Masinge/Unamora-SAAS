import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '../../components/Button';
import Input from '../../components/Input';
import Textarea from '../../components/Textarea';
import Card from '../../components/Card';
import './BookingFlow.css';

const CreateJobRequest: React.FC = () => {
  const navigate = useNavigate();
  const [step, setStep] = useState(1);
  const [formData, setFormData] = useState({
    serviceCategory: '',
    jobDescription: '',
    budget: 'flexible',
    budgetAmount: 0,
    datePreference: '',
    timePreference: '',
    location: '',
  });
  const [saving, setSaving] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async () => {
    setSaving(true);
    try {
      // API call to create job request
      setTimeout(() => {
        setSaving(false);
        navigate('/dashboard');
      }, 1000);
    } catch (error) {
      setSaving(false);
    }
  };

  return (
    <div className="booking-page">
      <div className="booking-header">
        <h1>Post a Job</h1>
        <p>Describe your project and get matched with professionals</p>
        <div className="progress-bar">
          <div className="progress" style={{ width: `${(step / 3) * 100}%` }} />
        </div>
      </div>

      <div className="booking-content">
        <Card variant="elevated">
          {step === 1 && (
            <>
              <h2>What service do you need?</h2>
              <select name="serviceCategory" value={formData.serviceCategory} onChange={handleChange} className="select-input">
                <option value="">Select a service</option>
                <option value="plumbing">Plumbing</option>
                <option value="electrical">Electrical</option>
                <option value="carpentry">Carpentry</option>
                <option value="general">General Repairs</option>
              </select>
              <Textarea
                label="Describe your project"
                name="jobDescription"
                value={formData.jobDescription}
                onChange={handleChange}
                fullWidth
                characterLimit={1000}
                rows={5}
                helperText="Be specific about what you need done"
              />
              <div className="step-actions">
                <Button variant="primary" size="medium" onClick={() => setStep(2)} fullWidth>
                  Next
                </Button>
              </div>
            </>
          )}

          {step === 2 && (
            <>
              <h2>When and where?</h2>
              <Input
                label="Preferred Date"
                name="datePreference"
                type="date"
                value={formData.datePreference}
                onChange={handleChange}
                fullWidth
              />
              <Input
                label="Preferred Time"
                name="timePreference"
                type="time"
                value={formData.timePreference}
                onChange={handleChange}
                fullWidth
              />
              <Input
                label="Location"
                name="location"
                value={formData.location}
                onChange={handleChange}
                fullWidth
                placeholder="Street address"
              />
              <div className="step-actions">
                <Button variant="secondary" size="medium" onClick={() => setStep(1)}>
                  Back
                </Button>
                <Button variant="primary" size="medium" onClick={() => setStep(3)}>
                  Next
                </Button>
              </div>
            </>
          )}

          {step === 3 && (
            <>
              <h2>What's your budget?</h2>
              <div className="budget-options">
                <label className="budget-option">
                  <input
                    type="radio"
                    name="budget"
                    value="flexible"
                    checked={formData.budget === 'flexible'}
                    onChange={handleChange}
                  />
                  <span>Flexible budget</span>
                </label>
                <label className="budget-option">
                  <input
                    type="radio"
                    name="budget"
                    value="specific"
                    checked={formData.budget === 'specific'}
                    onChange={handleChange}
                  />
                  <span>Specific budget</span>
                </label>
              </div>
              {formData.budget === 'specific' && (
                <Input
                  label="Budget (R)"
                  name="budgetAmount"
                  type="number"
                  value={formData.budgetAmount}
                  onChange={handleChange}
                  fullWidth
                />
              )}
              <div className="step-actions">
                <Button variant="secondary" size="medium" onClick={() => setStep(2)}>
                  Back
                </Button>
                <Button
                  variant="primary"
                  size="medium"
                  onClick={handleSubmit}
                  isLoading={saving}
                  fullWidth
                >
                  Post Job
                </Button>
              </div>
            </>
          )}
        </Card>
      </div>
    </div>
  );
};

export default CreateJobRequest;
