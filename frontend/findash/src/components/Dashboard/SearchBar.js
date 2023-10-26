import React, { useState } from 'react';
import axios from 'axios';

const SearchBar = ({ data, setStocks, existingStocks, handleRefreshEvent }) => {
  const [searchTerm, setSearchTerm] = useState('');

  const filteredData = data.filter((item) =>
  item.symbol.toLowerCase().startsWith(searchTerm.toLowerCase())
);

  const saveStock = async (stockId) => {
    const token = localStorage.getItem('token');
    const userId = localStorage.getItem('userId');
    try {
      const response = await axios.post('https://localhost:7222/Financial/SaveStock', {
        userId,
        stockId
      }, {
        headers: { Authorization: `Bearer ${token}` },
      });
      console.log(response.data)
      handleRefreshEvent()
    } catch (error) {
      console.error('API call failed:', error);
  }
};
  
  

  return (
    <div className="search-bar-container">
      <input
        type="text"
        placeholder="Search for a stock"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />
      {searchTerm && (
        <ul className="search-bar-list">
          {filteredData.map((item, index) => (
            <li key={index}>
              {item.symbol}
              {!existingStocks.includes(item.symbol) && (
                <button onClick={() => saveStock(item.id)}>+</button>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default SearchBar;