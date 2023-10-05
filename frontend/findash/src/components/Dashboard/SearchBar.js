import React, { useState } from 'react';

const SearchBar = ({ data, setStocks, existingStocks }) => {
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

  return (
    <div className="search-container">
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
                <button onClick={() => handleAddStock(item.symbol)}>+</button>
              )}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};

export default SearchBar;
