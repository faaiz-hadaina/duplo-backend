using MongoDB.Driver;
using DuploBackend.Models;
using DuploBackend.Models.DTOs;
using DuploBackend.Data;
using DuploBackend.Utils;
using System.Text.Json;
using System.Text;

namespace DuploBackend.Services;

public class OrderService
{
    private readonly MongoDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        MongoDbContext context,
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OrderService> logger)
    {
        _context = context;
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<(Transaction, HttpResponseMessage)> ProcessOrder(Transaction orderData)
    {
        await _context.Transactions.InsertOneAsync(orderData);

        var taxPayload = new TaxPayload
        {
            OrderId = orderData.Id!,
            PlatformCode = "022",
            OrderAmount = orderData.Amount
        };

        var json = JsonSerializer.Serialize(taxPayload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var taxUrl = _configuration["TaxUrl"] ?? "";
        var taxApiResponse = await _httpClient.PostAsync(taxUrl, content);

        return (orderData, taxApiResponse);
    }

    public async Task<decimal> CalculateCreditScore(int businessID)
    {
        var transactions = await _context.Transactions
            .Find(t => t.BusinessID == businessID)
            .ToListAsync();

        var totalAmount = transactions.Sum(t => t.Amount);
        var totalOrders = transactions.Count;

        return CalculateScoreUtils.CalculateCreditScore(totalAmount, totalOrders);
    }
}
