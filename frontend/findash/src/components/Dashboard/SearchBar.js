import React, { useState } from 'react';
import axios from 'axios';

const SearchBar = ({ data, setStocks, existingStocks, handleNewStock }) => {
  const [searchTerm, setSearchTerm] = useState('');

  const filteredData = data.filter((item) =>
  item.symbol.toLowerCase().startsWith(searchTerm.toLowerCase())
);


const handleAddStock = (stockSymbol) => {
    const stockObject = data.find(stock => stock.symbol === stockSymbol);
    if (stockObject) {
      setStocks((prevStocks) => [...prevStocks, stockObject]);
    }
  };

  const saveStock = async (stockId) => {
    const token = localStorage.getItem('token');
    const userId = localStorage.getItem('userId'); // Assuming you have saved userId in local storage
    try {
      const response = await axios.post('https://localhost:7222/Financial/SaveStock', {
        userId,
        stockId
      }, {
        headers: { Authorization: `Bearer ${token}` },
      });
      // Handle the response here. Maybe update the list of existing stocks?
      handleNewStock()
      //setStocks([...existingStocks, { id: stockId, symbol: stock.symbol }]);
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