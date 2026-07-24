import React from 'react';
import { useNavigate } from 'react-router-dom';

const NotFound: React.FC = () => {
  const navigate = useNavigate();

  return (
    <section className="not-found">
      <div className="not-found-content">
        <h1>404</h1>
        <h2>Page not found</h2>
        <p>The page you're looking for doesn't exist or has been moved.</p>
        <button className="button" onClick={() => navigate('/')}>
          Go home
        </button>
      </div>
    </section>
  );
};

export default NotFound;
