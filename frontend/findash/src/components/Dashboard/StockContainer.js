import React from 'react';
import axios from 'axios';

export default function StockContainer({ stocks, handleRefreshEvent, handleSelectedStock }) {

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
      <h2>Followed Stocks</h2>
      <div>
        {/* Display your stocks */}
        {stocks.map((stock, index) => (
            <div key={index} className="stock-item" >
                <span>{stock.symbol}</span>
                <span className="stock-price">{stock.price}</span>
                <span className="stock-currency">{stock.currency}</span>
                <button className="select-button" onClick={() => handleSelectedStock(stock)}>Select</button>
                <button className="remove-button" onClick={() => removeStock(stock.id)}>-</button>
       </div>
        ))}
      </div>
      
    </div>
  );
}