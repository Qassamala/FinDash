import React from 'react';
import axios from 'axios';
import StockContainer from '../components/Dashboard/StockContainer';
import MarketDataContainer from '../components/Dashboard/MarketDataContainer';

export default function UserHomePage() {

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
      <button onClick={handleLogout}>Log Out</button>
      <button onClick={handleTest}>Test Financial Endpoint</button>

      <h1>User Home Page</h1>
      <div>
        <StockContainer />
        <MarketDataContainer title="Market News" />
        <MarketDataContainer title="Currency" />
        <MarketDataContainer title="US 10 Year Bond Yields" />
        <MarketDataContainer title="Indices" />
      </div>
    </div>
  );
}
