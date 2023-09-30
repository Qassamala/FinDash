import React, { useEffect } from 'react';
import {
  BrowserRouter as Router,
  Route,
  Routes,
  useNavigate
} from 'react-router-dom';
import LoginPage from './components/LoginPage';
import AdminHomePage from './components/AdminHomePage';
import NonAdminHomePage from './components/HomePage';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/admin" element={<AdminHomePage />} />
        <Route path="/home" element={<NonAdminHomePage />} />
        <Route path="*" element={<LoginPage />} />
      </Routes>
      <NavigationHandler />
    </Router>
  );
}

function NavigationHandler() {
  const navigate = useNavigate();
  const token = localStorage.getItem('token');
  const isAdmin = localStorage.getItem('isAdmin') === 'true';

  useEffect(() => {
    if (token) {
      if (isAdmin) {
        navigate("/admin");
      } else {
        navigate("/home");
      }
    }
  }, [token, isAdmin, navigate]);

  return null;
}

export default App;
