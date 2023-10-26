import React from 'react';

export default function DetailedStockView({ stock }) {
  return (
    <div className='detailed-stock-view'>
        {(!stock || Object.keys(stock).length === 0) ? null : 
        <div>
            <div>Symbol: {stock.symbol}</div>
            <div>Company Name: {stock.companyName}</div>
        </div>}
    </div>
  );
}