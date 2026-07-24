import React, { useState, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { api } from '../../services/api';

const ProfileSetup: React.FC = () => {
  const { user } = useAuth();
  const [step, setStep] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  if (!user) {
    navigate('/login');
    return null;
  }

  const isClient = user.role === 'Client';

  // Common profile fields
  const [commonData, setCommonData] = useState({
    photo: '',
    bio: '',
    phoneNumber: user.phoneNumber || '',
  });

  // Client-specific fields
  const [clientData, setClientData] = useState({
    defaultAddressLine1: '',
    defaultAddressLine2: '',
    city: '',
    province: '',
    postalCode: '',
    countryCode: 'ZA',
  });

  // Tradesperson-specific fields
  const [tradespersonData, setTradespersonData] = useState({
    businessName: '',
    headline: '',
    bio: '',
    yearsOfExperience: 0,
    baseLatitude: 0,
    baseLongitude: 0,
    serviceRadiusKm: 30,
    hourlyRateMin: 0,
    hourlyRateMax: 0,
  });

  const handleCommonChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setCommonData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleClientChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setClientData(prev => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleTradespersonChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setTradespersonData(prev => ({
      ...prev,
      [name]: isNaN(Number(value)) ? value : Number(value),
    }));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      if (isClient) {
        await api.updateClientProfile(user.id, {
          ...commonData,
          ...clientData,
        });
      } else {
        await api.updateTradespersonProfile(user.id, {
          ...commonData,
          ...tradespersonData,
        });
      }
      navigate('/dashboard');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to update profile');
    } finally {
      setLoading(false);
    }
  };

  return (
    <section className="profile-setup">
      <div className="setup-card">
        <h1>Complete your profile</h1>
        <p>Help us get to know you better</p>

        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit}>
          {step === 1 && (
            <>
              <h2>Basic Information</h2>

              <div className="form-group">
                <label htmlFor="bio">About you</label>
                <textarea
                  id="bio"
                  name="bio"
                  value={commonData.bio}
                  onChange={handleCommonChange}
                  placeholder="Tell us about yourself"
                  rows={4}
                />
              </div>

              {isClient ? (
                <>
                  <div className="form-group">
                    <label htmlFor="defaultAddressLine1">Address</label>
                    <input
                      id="defaultAddressLine1"
                      name="defaultAddressLine1"
                      value={clientData.defaultAddressLine1}
                      onChange={handleClientChange}
                      placeholder="Street address"
                    />
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label htmlFor="city">City</label>
                      <input
                        id="city"
                        name="city"
                        value={clientData.city}
                        onChange={handleClientChange}
                        placeholder="Johannesburg"
                      />
                    </div>

                    <div className="form-group">
                      <label htmlFor="province">Province</label>
                      <input
                        id="province"
                        name="province"
                        value={clientData.province}
                        onChange={handleClientChange}
                        placeholder="Gauteng"
                      />
                    </div>
                  </div>

                  <div className="form-group">
                    <label htmlFor="postalCode">Postal Code</label>
                    <input
                      id="postalCode"
                      name="postalCode"
                      value={clientData.postalCode}
                      onChange={handleClientChange}
                      placeholder="2196"
                    />
                  </div>
                </>
              ) : (
                <>
                  <div className="form-group">
                    <label htmlFor="businessName">Business Name</label>
                    <input
                      id="businessName"
                      name="businessName"
                      value={tradespersonData.businessName}
                      onChange={handleTradespersonChange}
                      placeholder="John's Express Plumbing"
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="headline">Professional Headline</label>
                    <input
                      id="headline"
                      name="headline"
                      value={tradespersonData.headline}
                      onChange={handleTradespersonChange}
                      placeholder="Master Plumber - 24/7 Leak Expert"
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="bio">About Your Business</label>
                    <textarea
                      id="bio"
                      name="bio"
                      value={tradespersonData.bio}
                      onChange={handleTradespersonChange}
                      placeholder="Describe your experience and services"
                      rows={4}
                    />
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label htmlFor="yearsOfExperience">Years of Experience</label>
                      <input
                        id="yearsOfExperience"
                        name="yearsOfExperience"
                        type="number"
                        value={tradespersonData.yearsOfExperience}
                        onChange={handleTradespersonChange}
                        placeholder="10"
                      />
                    </div>

                    <div className="form-group">
                      <label htmlFor="serviceRadiusKm">Service Radius (km)</label>
                      <input
                        id="serviceRadiusKm"
                        name="serviceRadiusKm"
                        type="number"
                        value={tradespersonData.serviceRadiusKm}
                        onChange={handleTradespersonChange}
                        placeholder="30"
                      />
                    </div>
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label htmlFor="hourlyRateMin">Hourly Rate From (R)</label>
                      <input
                        id="hourlyRateMin"
                        name="hourlyRateMin"
                        type="number"
                        value={tradespersonData.hourlyRateMin}
                        onChange={handleTradespersonChange}
                        placeholder="350"
                      />
                    </div>

                    <div className="form-group">
                      <label htmlFor="hourlyRateMax">Hourly Rate To (R)</label>
                      <input
                        id="hourlyRateMax"
                        name="hourlyRateMax"
                        type="number"
                        value={tradespersonData.hourlyRateMax}
                        onChange={handleTradespersonChange}
                        placeholder="550"
                      />
                    </div>
                  </div>

                  <button
                    type="button"
                    className="button"
                    onClick={() => setStep(2)}
                  >
                    Next: Skills & Verification
                  </button>
                </>
              )}

              {isClient && (
                <button type="submit" className="button" disabled={loading}>
                  {loading ? 'Saving...' : 'Complete Profile'}
                </button>
              )}
            </>
          )}

          {step === 2 && !isClient && (
            <>
              <h2>Skills & Verification</h2>
              <p>Select your primary trades and skills (you can add more later)</p>

              <div className="form-group">
                <label>Primary Service Areas</label>
                <div className="checkbox-group">
                  <label className="checkbox">
                    <input type="checkbox" defaultChecked />
                    Plumbing
                  </label>
                  <label className="checkbox">
                    <input type="checkbox" />
                    Electrical
                  </label>
                  <label className="checkbox">
                    <input type="checkbox" />
                    Carpentry
                  </label>
                  <label className="checkbox">
                    <input type="checkbox" />
                    General Repairs
                  </label>
                </div>
              </div>

              <div className="form-actions">
                <button
                  type="button"
                  className="button button-secondary"
                  onClick={() => setStep(1)}
                >
                  Back
                </button>
                <button type="submit" className="button" disabled={loading}>
                  {loading ? 'Saving...' : 'Complete Profile'}
                </button>
              </div>
            </>
          )}
        </form>
      </div>
    </section>
  );
};

export default ProfileSetup;
