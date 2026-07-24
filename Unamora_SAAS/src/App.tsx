import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import './App.css';

// Pages
import Landing from './pages/Landing';
import Login from './pages/auth/Login';
import Register from './pages/auth/Register';
import ProfileSetup from './pages/auth/ProfileSetup';
import Dashboard from './pages/Dashboard';
import Search from './pages/Search';
import NotFound from './pages/NotFound';
import Layout from './components/Layout';

// Profile Pages
import ProfileView from './pages/Profile/ProfileView';
import ProfileEdit from './pages/Profile/ProfileEdit';

// Booking Pages
import CreateJobRequest from './pages/Bookings/CreateJobRequest';
import MyBookings from './pages/Bookings/MyBookings';

// Feature Pages
import ChatList from './pages/Chat/ChatList';
import PaymentPage from './pages/Payments/PaymentPage';
import ReviewDisplay from './pages/Reviews/ReviewDisplay';

// Admin Pages
import AdminDashboard from './pages/Admin/AdminDashboard';

// Protected Route Component
const ProtectedRoute = ({ children, requiredRole }: { children: JSX.Element; requiredRole?: string }) => {
  const { user, isLoading } = useAuth();

  if (isLoading) {
    return <div className="loading">Loading...</div>;
  }

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (requiredRole && user.role !== requiredRole) {
    return <Navigate to="/dashboard" replace />;
  }

  return children;
};

function AppRoutes() {
  const { user } = useAuth();

  return (
    <Router>
      <Routes>
        <Route element={<Layout />}>
          {/* Auth Routes */}
          <Route path="/" element={!user ? <Landing /> : <Navigate to="/dashboard" />} />
          <Route path="/login" element={!user ? <Login /> : <Navigate to="/dashboard" />} />
          <Route path="/register" element={!user ? <Register /> : <Navigate to="/dashboard" />} />
          <Route path="/profile-setup" element={<ProtectedRoute><ProfileSetup /></ProtectedRoute>} />
          
          {/* Public Routes */}
          <Route path="/search" element={<Search />} />

          {/* Protected Routes */}
          <Route path="/dashboard" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />

          {/* Profile Routes */}
          <Route path="/profile/:userId" element={<ProtectedRoute><ProfileView /></ProtectedRoute>} />
          <Route path="/profile/edit" element={<ProtectedRoute><ProfileEdit /></ProtectedRoute>} />

          {/* Booking Routes */}
          <Route path="/post-job" element={<ProtectedRoute><CreateJobRequest /></ProtectedRoute>} />
          <Route path="/bookings" element={<ProtectedRoute><MyBookings /></ProtectedRoute>} />

          {/* Feature Routes */}
          <Route path="/chat" element={<ProtectedRoute><ChatList /></ProtectedRoute>} />
          <Route path="/payments" element={<ProtectedRoute><PaymentPage /></ProtectedRoute>} />
          <Route path="/reviews" element={<ProtectedRoute><ReviewDisplay /></ProtectedRoute>} />

          {/* Admin Routes */}
          <Route path="/admin/dashboard" element={<ProtectedRoute requiredRole="Admin"><AdminDashboard /></ProtectedRoute>} />

          {/* 404 Route */}
          <Route path="*" element={<NotFound />} />
        </Route>
      </Routes>
    </Router>
  );
}

function App() {
  return (
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  );
}

export default App;
