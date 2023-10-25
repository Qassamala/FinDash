import React, { useState } from 'react';
import axios from 'axios';

export default function StockContainer({ stocks, handleRefreshEvent }) {
  const [searchData, setSearchData] = useState([]);
  const [isSearchOpen, setIsSearchOpen] = useState(false);

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

  const removeStock = async (stockId) => {
    const token = localStorage.getItem('token');
    try {
      const response = await axios.delete(`https://localhost:7222/Financial/RemoveStock/${stockId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      console.log(response.data)
      handleRefreshEvent()
    } catch (error) {
      console.error('Error removing stock:', error);
    }
  };

  return (
    <div className="stock-container">

      <h2>Your Stocks</h2>
      <div>
        {/* Display your stocks */}
        {stocks.map((stock, index) => (
            <div key={index} className="stock-item">
                <span>{stock.symbol}</span>
                <span className="stock-price">{stock.price}</span>
                <button className="remove-button" onClick={() => removeStock(stock.id)}>-</button>
       </div>
        ))}
      </div>
      
    </div>
  );
}