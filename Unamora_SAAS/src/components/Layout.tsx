import React from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const Layout: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/');
  };

  return (
    <div className="app-shell">
      <header className="site-header">
        <a className="brand" href="/" aria-label="Unamora home">
          unamora<span>.</span>
        </a>
        <nav aria-label="Main navigation">
          <a href="/search">Find a pro</a>
          <a href="/#pricing">Pricing</a>
          <a href="/#help">Help centre</a>
        </nav>
        <div className="header-actions">
          {user ? (
            <>
              <span className="user-name">{user.firstName}</span>
              <button className="button button-small" onClick={handleLogout}>
                Sign out
              </button>
            </>
          ) : (
            <>
              <a href="/login">Sign in</a>
              <a className="button button-small" href="/register">Get started</a>
            </>
          )}
        </div>
      </header>
      <main>
        <Outlet />
      </main>
      <footer>
        <span>© 2026 Unamora</span>
        <div>
          <a href="/#terms">Terms</a>
          <a href="/#privacy">Privacy</a>
          <a href="/#contact">Contact</a>
        </div>
      </footer>
    </div>
  );
};

export default Layout;
