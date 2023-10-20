import React, { useState } from 'react';
import axios from 'axios';

export default function StockContainer() {
  const [stocks, setStocks] = useState([]);
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

  const removeStock = (stock) => {
    setStocks(stocks.filter((item) => item !== stock));
  };

  return (
    <div>

      <h2>Your Stocks</h2>
      <div>
        {/* Display your stocks */}
        {stocks.map((stock, index) => (
            <div key={index} className="stock-item">
                <span>{stock.symbol}</span>
                <span className="stock-price">{stock.price}</span>
                <button className="remove-button" onClick={() => removeStock(stock)}>-</button>
       </div>
        ))}
      </div>
      
    </div>
  );
}