import React, { useState, useEffect } from 'react';
import axios from 'axios';
import StockContainer from './StockContainer';
import MarketDataContainer from './MarketDataContainer';
import SearchBar from './SearchBar';

export default function UserHomePage() {
  const [stocks, setStocks] = useState([]);
  const [searchData, setSearchData] = useState([]);
  const [isSearchOpen, setIsSearchOpen] = useState(false);
  const [reload, setReload] = useState(false);

  useEffect(() => {
    // Load saved stocks when the page loads or when reload state changes
    const fetchSavedStocks = async () => {
      const token = localStorage.getItem('token');
      const userId = localStorage.getItem('userId');
      try {
        const response = await axios.get(`https://localhost:7222/Financial/SavedStocks/${userId}`, {
          headers: { Authorization: `Bearer ${token}` },
        });
        setStocks(response.data);
      } catch (error) {
        console.error('Error fetching saved stocks:', error);
      }
    };
    
    fetchSavedStocks();
  }, [reload]);

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('isAdmin');
    localStorage.removeItem('userId');
    window.location.href = '/';
  };

  const loadStocks = async () => {
    if (searchData.length === 0) {
      const token = localStorage.getItem('token');
      try {
        const response = await axios.get('https://localhost:7222/Financial/LoadStocks', {
          headers: { Authorization: `Bearer ${token}` },
        });
        setSearchData(response.data);
      } catch (error) {
        console.error('Error loading stocks:', error);
      }
    }
  };

  const toggleSearch = () => {
    setIsSearchOpen(!isSearchOpen);
    if (!isSearchOpen && searchData.length === 0) {
      loadStocks();
    }
  };

  const handleRefreshEvent = () => {
    // Your logic to add a new stock, then reload to update
    setReload(!reload);
  };

  return (
    <div className="dashboard">
      <div className="top-section">
        {isSearchOpen ? (
          <SearchBar data={searchData} setStocks={setStocks} existingStocks={stocks.map(stock => stock.symbol)} handleRefreshEvent={handleRefreshEvent} />
        ) : (
          <button onClick={toggleSearch}>+</button>
        )}
        <h2>FINDASH</h2>
        <button onClick={handleLogout}>Logout</button>
      </div>
      <div className="main-content">
        <StockContainer stocks={stocks} handleRefreshEvent={handleRefreshEvent} />
        <div className="details-and-news">
          <MarketDataContainer title="Market News" />
          <MarketDataContainer title="US 10 Year Bond Yields" />
        </div>
      </div>
    </div>
  );
}
