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

            string currency = GetCurrency(region);

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
                    StaticStockData staticStockData = new StaticStockData { Symbol = symbol, CompanyName = companyName, Currency =  currency};
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

    // Supports only SEK and USD so far
    private string GetCurrency(string region)
    {
        return region.Equals("ST") ? "SEK" : "USD";
    }

    internal async Task GetStockPrices(string region)
    {
        Console.WriteLine($"GetSTockPrices says region is {region}");

        // Retrieve all symbols and make external API call, update db Stockprice entity with result

        var stockData = _context.StaticStockData
                    .Where(symbol => symbol.Currency == GetCurrency(region))
                    .Select(g => g.Symbol)
                    .ToArray();

        string[] batch;

       // Console.WriteLine((stockData.Count() / 100) * 2);

        try
        {
            for (int i = 0; i < stockData.Count() / 50; i++)
            {
                batch = CreateBatch(stockData, i);
                string url = ConstructURL(batch, region);   // creates unique URL string
                var response = await MakeHTTPCallToYahoo(url);
                var parsed = DeserializeYahooResponse(response);
                await UpdatePricesInDatabase(parsed);
                //Thread.Sleep(300);
            }
        }
        catch (Exception)
        {
            throw new Exception("Unable to perform action");
        }

    }

    // Create batch with max 50 stocks for API call
    private string[] CreateBatch(string[] stockData, int i)
    {
        var batch = stockData
            .Skip(i * (50))
           .Take(50)
           .ToArray();

        Console.WriteLine(batch.Count());

        return batch;
    }

    private JsonElement.ArrayEnumerator DeserializeYahooResponse(string body)
    {
        var jsonDocument = JsonDocument.Parse(body);
        var root = jsonDocument.RootElement;
        var quoteResponse = root.GetProperty("quoteResponse");
        return quoteResponse.GetProperty("result").EnumerateArray();
    }

    private async Task UpdatePricesInDatabase(JsonElement.ArrayEnumerator parsed)
    {
        int counter = 0;

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
                counter++;

                await _context.StockPrices.AddAsync(stockPrice);
            }
        }
        Console.WriteLine(counter);
        await _context.SaveChangesAsync();
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

        Console.WriteLine(url);

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
                        Price = stockPrice.Price,
                        CompanyName = staticData.CompanyName,
                        Currency = staticData.Currency
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
                                CompanyName = us.StaticStockData.CompanyName,
                                Currency = us.StaticStockData.Currency,
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
                                Currency = us.Currency,
                                CompanyName = us.CompanyName
                            }).ToList();


        return stocksForUser.ToList();
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
