import React from 'react';
import axios from 'axios';

export default function AdminHomePage() {
  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('isAdmin');
    window.location.href = '/';
  };

  const handleTest = async () => {
    const token = localStorage.getItem('token');
    const userId = localStorage.getItem('userId'); // Get the user ID from local storage
    try {
      const response = await axios.get(`https://localhost:7222/Financial/${userId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      console.log(response.data);
    } catch (error) {
      console.error('API call failed:', error);
    }
  };

  return (
    <div>
      <h1>Admin Home Page</h1>
      <div></div>
      <button onClick={handleLogout}>Log Out</button>
      <button onClick={handleTest}>Test Financial Endpoint</button>
    </div>
  );
}
