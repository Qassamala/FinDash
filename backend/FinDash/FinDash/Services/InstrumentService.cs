using FinDash.Config;
using FinDash.Data;
using FinDash.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using FinDash.DTOs;
using FinDash.Helpers;

public class InstrumentService
{
    private readonly FinDashDbContext _context;
    private readonly IConfiguration _configuration;

    public InstrumentService(FinDashDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task AddStaticStockData(string region)
    {
        try
        {
            // File path is enabled via VS settings to be copied to output directory
            // So far only supporting ST and US
            string path = region.Equals("ST") ? _configuration[ConfigurationKeys.StaticStockDataSWE]!: _configuration[ConfigurationKeys.StaticStockDataUS]!;

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

    internal async Task GetStockPrices(string region)
    {
        // TODO retrieve all symbols and make external API call, update db Stockprice entity with result

        Console.WriteLine($"GetSTockPrices says region is {region}");

        var stockData = _context.StaticStockData
                    .Select(g => g.Symbol)
                    .ToArray();

        int symbolsFound = stockData.Count();

        Console.WriteLine("Symbols Found: " + symbolsFound);

        string[] batch;

        try
        {
            for (int i = 0; i < symbolsFound / 99; i++)
            {
                batch = CreateBatch(stockData, i, region);
                string url = ConstructURL(batch, region);   // creates unique URL string
                var response = await MakeHTTPCallToYahoo(url);
                var parsed = DeserializeYahooResponse(response);
                UpdatePricesInDatabase(parsed);
                Thread.Sleep(300);
            }
        }
        catch (Exception)
        {
            throw new Exception("Unable to perform action");
        }

    }

    // Create batch with max 99 stocks for API call
    private string[] CreateBatch(string[] stockData, int i, string region)
    {
        var batch = stockData
            .Skip(i * 99)
           .Take(99)
           .ToArray();

        return batch;
    }

    private JsonElement.ArrayEnumerator DeserializeYahooResponse(string body)
    {
        var jsonDocument = JsonDocument.Parse(body);
        var root = jsonDocument.RootElement;
        var quoteResponse = root.GetProperty("quoteResponse");
        return quoteResponse.GetProperty("result").EnumerateArray();
    }

    private void UpdatePricesInDatabase(JsonElement.ArrayEnumerator parsed)
    {
        Console.WriteLine(parsed.ElementAt(0));

        foreach (var item in parsed)
        {
            StaticStockData stock = _context.StaticStockData.SingleOrDefault(s => s.Symbol == item.GetProperty("symbol").GetString())!;

            if (stock != null && item.TryGetProperty("regularMarketPrice", out var priceElement))
            {
                StockPrice stockPrice = new StockPrice
                {
                    Timestamp = DateTime.Now,
                    Price = priceElement.GetDecimal(),
                    StaticStockDataId = stock.Id
                };

                _context.StockPrices.AddAsync(stockPrice);
            }
        }
        _context.SaveChangesAsync();
    }


    public async Task<string> MakeHTTPCallToYahoo(string url)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
            Headers =
            {
                { "X-RapidAPI-Key", _configuration[ConfigurationKeys.YahooAPIKey] },
                { "X-RapidAPI-Host",  _configuration[ConfigurationKeys.YahooAPIHost] },
            },
        };

        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine(body);
            return body;
        }
    }

    // Construct URL with max 99 stocks
    private static string ConstructURL(string[] result, string region)
    {
        string url = "https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/v2/get-quotes?region=" + region + "&symbols=";

        foreach (var item in result)
        {
            url = url + item + ",";
        }

        return url;
    }

    internal List<StockViewDTO> LoadStocks()
    {
        List<StockViewDTO> stockList = new List<StockViewDTO>();

        var query = from stockPrice in _context.StockPrices
                    join staticData in _context.StaticStockData
                    on stockPrice.StaticStockDataId equals staticData.Id
                    select new StockViewDTO
                    {
                        Id = staticData.Id,
                        Symbol = staticData.Symbol,
                        LastUpdated = stockPrice.Timestamp,
                        Price = stockPrice.Price
                    };

        return query.ToList();
    }

    internal List<StockViewDTO> LoadSavedStocks(int id)
    {
        List<StockViewDTO> stockList = new List<StockViewDTO>();

        var stocksForUser = _context.UserStocks
                            .Where(us => us.UserId == id)
                            .Include(us => us.StaticStockData)
                            .ThenInclude(ssd => ssd.StockPrices)
                            .Select(us => new {
                                Id = us.Id,
                                Symbol = us.StaticStockData.Symbol,
                                LatestStockPrice = us.StaticStockData.StockPrices
                                                .OrderByDescending(sp => sp.Timestamp)
                                                .FirstOrDefault()
                            })
                            .Select(us => new StockViewDTO
                            {
                                Id = us.Id,
                                Symbol = us.Symbol,
                                LastUpdated = us.LatestStockPrice.Timestamp,
                                Price = us.LatestStockPrice.Price,
                                Currency = getCurrency(us.Symbol)
                            }).ToList();


        return stocksForUser.ToList();
    }

    private static string getCurrency(string symbol)
    {

        return symbol.Contains(".ST") ? "SEK" : "USD";
    }

    internal async Task AddStockToUser(AddStockDTO addStockDTO)
    {
        var userStock = new UserStock
        {
            UserId = addStockDTO.UserId,
            StaticStockDataId = addStockDTO.StockId
        };

        await _context.UserStocks.AddAsync(userStock);
        await _context.SaveChangesAsync();
    }

    internal async Task RemoveSavedStock(int id)
    {
        // Fetch the record to be deleted
        var userStock = await _context.UserStocks.FindAsync(id);

        // Check if the record exists
        if (userStock == null)
        {
            throw new Exception("Id not found.");
        }

        // Delete the record
        _context.UserStocks.Remove(userStock);
        await _context.SaveChangesAsync();
    }
}
