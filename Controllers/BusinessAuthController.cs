using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using DuploBackend.Models;
using DuploBackend.Models.DTOs;
using DuploBackend.Services;
using DuploBackend.Data;
using DuploBackend.Utils;

namespace DuploBackend.Controllers;

[ApiController]
[Route("api/business")]
public class BusinessAuthController : ControllerBase
{
    private readonly MongoDbContext _context;
    private readonly AuthService _authService;
    private readonly GenerateRandomUtils _randomUtils;
    private readonly ILogger<BusinessAuthController> _logger;

    public BusinessAuthController(
        MongoDbContext context,
        AuthService authService,
        GenerateRandomUtils randomUtils,
        ILogger<BusinessAuthController> logger)
    {
        _context = context;
        _authService = authService;
        _randomUtils = randomUtils;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (request.Password.Length < 6)
            {
                return BadRequest(new { msg = "Password must be at least 6 characters.", success = false });
            }

            var existingBusiness = await _context.Businesses
                .Find(b => b.Name == request.Name)
                .FirstOrDefaultAsync();

            if (existingBusiness != null)
            {
                return BadRequest(new { message = $"{request.Name} is already registered", success = false });
            }

            var businessID = await _randomUtils.GenerateUniqueBusinessID();
            var hashedPassword = _authService.HashPassword(request.Password);

            var newBusiness = new Business
            {
                Name = request.Name,
                Password = hashedPassword,
                BusinessID = businessID,
                TotalAmount = 0
            };

            await _context.Businesses.InsertOneAsync(newBusiness);

            return CreatedAtAction(nameof(Register), new
            {
                msg = "Registration Success!",
                business = new
                {
                    id = newBusiness.Id,
                    name = newBusiness.Name,
                    businessID = newBusiness.BusinessID,
                    password = ""
                },
                success = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Registration failed: {ex.Message}");
            return StatusCode(500, new { message = ex.Message, success = false });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var business = await _context.Businesses
                .Find(b => b.Name == request.Name)
                .FirstOrDefaultAsync();

            if (business == null)
            {
                return BadRequest(new { message = $"{request.Name} does not exist", success = false });
            }

            var isMatch = _authService.VerifyPassword(request.Password, business.Password);

            if (!isMatch)
            {
                return NotFound(new { message = "Invalid Credentials", success = false });
            }

            var token = _authService.GenerateJwtToken(business.Id!, business.Name);

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login failed: {ex.Message}");
            return StatusCode(500, new { message = ex.Message, success = false });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBusinesses([FromQuery] int? page, [FromQuery] int? limit)
    {
        try
        {
            var queryable = _context.Businesses.Find(_ => true);

            if (page.HasValue && limit.HasValue)
            {
                var skip = (page.Value - 1) * limit.Value;
                var businesses = await queryable.Skip(skip).Limit(limit.Value).ToListAsync();
                return Ok(businesses);
            }

            var allBusinesses = await queryable.ToListAsync();
            return Ok(allBusinesses);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching businesses: {ex.Message}");
            return StatusCode(500, new { error = "Internal Server Error" });
        }
    }
}
