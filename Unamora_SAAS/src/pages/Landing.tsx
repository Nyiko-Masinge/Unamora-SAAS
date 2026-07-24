import React, { useState, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Landing: React.FC = () => {
  const [query, setQuery] = useState('');
  const navigate = useNavigate();

  const handleSearch = (e: FormEvent) => {
    e.preventDefault();
    navigate(`/search?q=${encodeURIComponent(query)}`);
  };

  return (
    <>
      <section className="hero-section">
        <p className="eyebrow">Trusted local services, made simple</p>
        <h1>Find the right person<br />for the job.</h1>
        <p className="hero-copy">From urgent repairs to projects you have been planning, Unamora connects you with verified professionals who are ready to help.</p>
        <form className="search-box" onSubmit={handleSearch}>
          <label htmlFor="search">What do you need help with?</label>
          <div>
            <input
              id="search"
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              placeholder='Try "emergency plumber near me"'
            />
            <button className="button" type="submit">Search</button>
          </div>
        </form>
        <p className="trust-line">✓ Verified profiles &nbsp; ✓ Secure bookings &nbsp; ✓ Clear communication</p>
      </section>
      <section className="content-section">
        <div className="section-heading">
          <p className="eyebrow">One platform, every step</p>
          <h2>Built for work that matters.</h2>
        </div>
        <div className="feature-grid">
          <Feature number="01" title="Find with confidence" text="Search by service, location, availability, and the proof that matters to you." />
          <Feature number="02" title="Book without guesswork" text="Keep quotes, booking details, secure payments, and updates in one clear timeline." />
          <Feature number="03" title="Grow a trusted business" text="Give skilled tradespeople the tools to showcase work and manage every opportunity." />
        </div>
      </section>
      <section className="cta-section">
        <div>
          <p className="eyebrow">Ready when you are</p>
          <h2>Bring your next project to life.</h2>
        </div>
        <button className="button" onClick={() => navigate('/register')}>Create your account</button>
      </section>
    </>
  );
};

interface FeatureProps {
  number: string;
  title: string;
  text: string;
}

const Feature: React.FC<FeatureProps> = ({ number, title, text }) => (
  <article className="feature">
    <span>{number}</span>
    <h3>{title}</h3>
    <p>{text}</p>
  </article>
);

export default Landing;
