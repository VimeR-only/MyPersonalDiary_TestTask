import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';

import Login from './components/Login';
import Home from './components/Home';
import RegisterPage from './components/Register';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [username, setUsername] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const checkSession = async () => {
      try {
        const response = await fetch('https://localhost:7018/api/Auth/check-session', {
          method: 'POST',
          credentials: 'include',
        });

        if (response.ok) {
          const data = await response.json();
          setIsLoggedIn(true);
          setUsername(data.username ?? '');
        }
      } catch (error) {
        console.error('Session check failed:', error);
      } finally {
        setLoading(false);
      }
    };

    checkSession();
  }, []);

  const handleLogin = (user) => {
    setUsername(user);
    setIsLoggedIn(true);
  };

  const handleLogout = () => {
    setUsername('');
    setIsLoggedIn(false);

    fetch('https://localhost:7018/api/Auth/logout', {
      method: 'POST',
      credentials: 'include',
    });
  };

  if (loading) return <p>Loading...</p>;

  return (
    <Router>
      <div style={styles.container}>
        <h1>My Personal Diary</h1>

        <Routes>
          <Route
            path="/"
            element={
              isLoggedIn
                ? <Home username={username} onLogout={handleLogout} />
                : <Navigate to="/login" replace />
            }
          />
          <Route
            path="/login"
            element={
              isLoggedIn
                ? <Navigate to="/" replace />
                : <Login onLogin={handleLogin} />
            }
          />
          <Route path="/register/:inviteId" element={<RegisterPage />} />
        </Routes>
      </div>
    </Router>
  );
}

const styles = {
  container: {
    textAlign: 'center',
    marginTop: '100px',
    fontFamily: 'sans-serif',
  },
};

export default App;
