using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using DuploBackend.Models;
using DuploBackend.Models.DTOs;
using DuploBackend.Services;
using DuploBackend.Data;

namespace DuploBackend.Controllers;

[ApiController]
[Route("api")]
public class OrderController : ControllerBase
{
    private readonly MongoDbContext _context;
    private readonly OrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        MongoDbContext context,
        OrderService orderService,
        ILogger<OrderController> logger)
    {
        _context = context;
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost("orders")]
    [Authorize(Policy = "DepartmentPolicy")]
    public async Task<IActionResult> ProcessOrder([FromBody] OrderRequest request)
    {
        try
        {
            var departmentIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(departmentIdClaim))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var department = await _context.Departments
                .Find(d => d.Id == departmentIdClaim)
                .FirstOrDefaultAsync();

            if (department == null)
            {
                return Unauthorized(new { message = "Department not found" });
            }

            var orderData = new Transaction
            {
                BusinessID = department.BusinessID,
                Amount = request.Amount,
                Date = request.Date,
                Status = request.Status
            };

            var (savedOrder, taxApiResponse) = await _orderService.ProcessOrder(orderData);

            var taxResponseContent = await taxApiResponse.Content.ReadAsStringAsync();
            _logger.LogInformation($"Tax API response: {taxResponseContent}");
            _logger.LogInformation("Order processed successfully");

            var business = await _context.Businesses
                .Find(b => b.BusinessID == department.BusinessID)
                .FirstOrDefaultAsync();

            if (business != null)
            {
                business.TotalAmount += request.Amount;
                await _context.Businesses.ReplaceOneAsync(b => b.Id == business.Id, business);
            }

            return Ok(new { message = "Order processed successfully", savedOrder });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing order: {ex.Message}");
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }

    [HttpGet("credit-score")]
    [Authorize(Policy = "BusinessPolicy")]
    public async Task<IActionResult> GetCreditScore()
    {
        try
        {
            var businessIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(businessIdClaim))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var business = await _context.Businesses
                .Find(b => b.Id == businessIdClaim)
                .FirstOrDefaultAsync();

            if (business == null)
            {
                return Unauthorized(new { message = "Business not found" });
            }

            var creditScore = await _orderService.CalculateCreditScore(business.BusinessID);

            return Ok(new { creditScore });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching credit score: {ex.Message}");
            return StatusCode(500, new { message = $"Internal Server Error: {ex.Message}" });
        }
    }

    [HttpGet("order-details/{businessID}")]
    public async Task<IActionResult> GetOrderDetails(int businessID, [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        try
        {
            var startOfDay = DateTime.Today;

            var filter = Builders<Transaction>.Filter.And(
                Builders<Transaction>.Filter.Eq(t => t.BusinessID, businessID),
                Builders<Transaction>.Filter.Eq(t => t.Status, "out-of-stock")
            );

            var transactions = await _context.Transactions
                .Find(filter)
                .ToListAsync();

            if (transactions.Count == 0)
            {
                return NotFound(new { error = "No orders found" });
            }

            var totalOrders = transactions.Count;
            var totalAmount = transactions.Sum(t => t.Amount);

            var todayTransactions = transactions.Where(t => t.Date >= startOfDay).ToList();
            var todayTotalOrders = todayTransactions.Count;
            var todayTotalAmount = todayTransactions.Sum(t => t.Amount);

            return Ok(new
            {
                totalOrders,
                totalAmount,
                todayTotalOrders,
                todayTotalAmount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching order details: {ex.Message}");
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
