import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import Button from '../../components/Button';
import Input from '../../components/Input';
import Textarea from '../../components/Textarea';
import Card from '../../components/Card';
import './ProfileEdit.css';

const ProfileEdit: React.FC = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    firstName: user?.firstName || '',
    lastName: user?.lastName || '',
    headline: '',
    bio: '',
    businessName: '',
    yearsOfExperience: 0,
    hourlyRateMin: 0,
    hourlyRateMax: 0,
    serviceRadius: 30,
  });
  const [saving, setSaving] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: isNaN(Number(value)) ? value : Number(value),
    }));
  };

  const handleSave = async () => {
    setSaving(true);
    try {
      // API call to update profile
      setTimeout(() => {
        setSaving(false);
        navigate('/profile');
      }, 1000);
    } catch (error) {
      setSaving(false);
    }
  };

  return (
    <div className="profile-edit">
      <div className="edit-header">
        <h1>Edit Profile</h1>
        <p>Keep your profile information up to date</p>
      </div>

      <div className="edit-content">
        <Card variant="elevated" className="edit-section">
          <h2>Personal Information</h2>
          <div className="form-row">
            <Input
              label="First Name"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              fullWidth
            />
            <Input
              label="Last Name"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              fullWidth
            />
          </div>
        </Card>

        <Card variant="elevated" className="edit-section">
          <h2>Professional Information</h2>
          <Input
            label="Business/Professional Name"
            name="businessName"
            value={formData.businessName}
            onChange={handleChange}
            fullWidth
          />
          <Input
            label="Professional Headline"
            name="headline"
            value={formData.headline}
            onChange={handleChange}
            fullWidth
            helperText="e.g., Master Plumber - 15+ Years Experience"
          />
          <Textarea
            label="About You"
            name="bio"
            value={formData.bio}
            onChange={handleChange}
            fullWidth
            characterLimit={500}
            rows={5}
          />
        </Card>

        <Card variant="elevated" className="edit-section">
          <h2>Service Details</h2>
          <Input
            label="Years of Experience"
            name="yearsOfExperience"
            type="number"
            value={formData.yearsOfExperience}
            onChange={handleChange}
            fullWidth
          />
          <Input
            label="Service Radius (km)"
            name="serviceRadius"
            type="number"
            value={formData.serviceRadius}
            onChange={handleChange}
            fullWidth
          />
          <div className="form-row">
            <Input
              label="Hourly Rate From (R)"
              name="hourlyRateMin"
              type="number"
              value={formData.hourlyRateMin}
              onChange={handleChange}
              fullWidth
            />
            <Input
              label="Hourly Rate To (R)"
              name="hourlyRateMax"
              type="number"
              value={formData.hourlyRateMax}
              onChange={handleChange}
              fullWidth
            />
          </div>
        </Card>

        <div className="edit-actions">
          <Button
            variant="secondary"
            size="medium"
            onClick={() => navigate('/profile')}
          >
            Cancel
          </Button>
          <Button
            variant="primary"
            size="medium"
            onClick={handleSave}
            isLoading={saving}
          >
            Save Changes
          </Button>
        </div>
      </div>
    </div>
  );
};

export default ProfileEdit;
