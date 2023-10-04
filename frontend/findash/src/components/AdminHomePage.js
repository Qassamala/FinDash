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

  const handleAddStaticStockData = async () => {
    const token = localStorage.getItem('token');
    try {
      const response = await axios.post('https://localhost:7222/Financial/AddStaticStockData', {}, {
        headers: { Authorization: `Bearer ${token}` },
      });
      console.log('Static stock data added successfully:', response.data);
    } catch (error) {
      console.error('An error occurred while adding static stock data:', error);
    }
  };

  const handleUpdateDb = async (value) => {
    const token = localStorage.getItem('token');
    try {
      const response = await axios.post(`https://localhost:7222/Financial/RetrieveStockPrices?region=${value}`, 
      {}, // Empty object since you're not sending any data in the request body
      {
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
      <button onClick={handleAddStaticStockData}>Add Static Stock Data</button>
      <button onClick={() => handleUpdateDb('US')}>Update DB for US</button>
      <button onClick={() => handleUpdateDb('ST')}>Update DB for ST</button>
    </div>
  );
}
