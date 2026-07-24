import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { api } from '../services/api';

const Search: React.FC = () => {
  const [searchParams] = useSearchParams();
  const [results, setResults] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const query = searchParams.get('q') || '';

  useEffect(() => {
    if (query) {
      loadResults();
    }
  }, [query]);

  const loadResults = async () => {
    setLoading(true);
    try {
      const response = await api.searchTradespersons({ keyword: query });
      setResults(response.data || []);
    } catch (error) {
      console.error('Search failed:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <section className="search-page">
      <p className="eyebrow">Smart search</p>
      <h1>Professionals matched to your needs.</h1>
      <p className="dashboard-copy">
        {query ? `Showing the best available matches for "${query}".` : 'Use plain language to find a verified professional.'}
      </p>

      {loading && <p className="loading">Searching...</p>}

      {results.length === 0 && !loading && (
        <div className="no-results">
          <p>No professionals found matching your search.</p>
          <p>Try adjusting your search terms or browse by category.</p>
        </div>
      )}

      <div className="result-grid">
        {results.map((professional, index) => (
          <article className="result" key={index}>
            <span className="avatar">{professional.firstName?.charAt(0)}{professional.lastName?.charAt(0)}</span>
            <div>
              <h2>{professional.businessName || `${professional.firstName} ${professional.lastName}`}</h2>
              <p>{professional.headline} · {professional.city}</p>
              <small>Rating: {professional.rating}/5.0 ({professional.reviewCount} reviews)</small>
            </div>
            <button className="button button-small">View profile</button>
          </article>
        ))}
      </div>
    </section>
  );
};

export default Search;
