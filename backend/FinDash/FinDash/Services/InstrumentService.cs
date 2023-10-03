using FinDash.Config;
using FinDash.Data;
using FinDash.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class InstrumentService
{
    private readonly FinDashDbContext _context;
    private readonly IConfiguration _configuration;

    public InstrumentService(FinDashDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task AddStaticStockData()
    {
        try
        {
            // File path is enabled via VS settings to be copied to output directory 
            string path = _configuration[ConfigurationKeys.StaticStockData]!;

            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Static Stock data file not found.");
            }

            // Read the lines from static stock data file
            string[] inputFileStocks = File.ReadAllLines(path);

            List<StaticStockData> uniqueStockDataList = new List<StaticStockData>();

            // Existing symbols in the database
            var existingSymbols = await _context.StaticStockData.Select(s => s.Symbol).ToListAsync();

            foreach (var item in inputFileStocks)
            {
                string[] lines = item.Split('\t');
                string symbol = lines[0];

                // Check if symbol already exists in the database
                if (!existingSymbols.Contains(symbol))
                {
                    string companyName = lines[1];  // Also extract the company name
                    StaticStockData staticStockData = new StaticStockData { Symbol = symbol, CompanyName = companyName };
                    uniqueStockDataList.Add(staticStockData);
                }
            }

            // Bulk insert at the end
            if (uniqueStockDataList.Count > 0)
            {

                await _context.StaticStockData.AddRangeAsync(uniqueStockDataList);
                await _context.SaveChangesAsync();
            }
            else throw new Exception("No new stocks to add");

        }
        catch (Exception e)
        {
            // Log the exception
            Console.WriteLine(e);
            throw new Exception("Could not perform action.");
        }
    }
}
