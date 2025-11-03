using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using DuploBackend.Models;
using DuploBackend.Models.DTOs;
using DuploBackend.Services;
using DuploBackend.Data;
using DuploBackend.Utils;

namespace DuploBackend.Controllers;

[ApiController]
[Route("api/department")]
public class DepartmentAuthController : ControllerBase
{
    private readonly MongoDbContext _context;
    private readonly AuthService _authService;
    private readonly GenerateRandomUtils _randomUtils;
    private readonly ILogger<DepartmentAuthController> _logger;

    public DepartmentAuthController(
        MongoDbContext context,
        AuthService authService,
        GenerateRandomUtils randomUtils,
        ILogger<DepartmentAuthController> logger)
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

            var existingDepartment = await _context.Departments
                .Find(d => d.Name == request.Name)
                .FirstOrDefaultAsync();

            if (existingDepartment != null)
            {
                return BadRequest(new { message = $"{request.Name} is already registered", success = false });
            }

            var businessID = await _randomUtils.GenerateUniqueBusinessID();
            var hashedPassword = _authService.HashPassword(request.Password);

            var newDepartment = new Department
            {
                Name = request.Name,
                Password = hashedPassword,
                BusinessID = businessID
            };

            await _context.Departments.InsertOneAsync(newDepartment);

            return CreatedAtAction(nameof(Register), new
            {
                msg = "Registration Success!",
                department = new
                {
                    id = newDepartment.Id,
                    name = newDepartment.Name,
                    businessID = newDepartment.BusinessID,
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
            var department = await _context.Departments
                .Find(d => d.Name == request.Name)
                .FirstOrDefaultAsync();

            if (department == null)
            {
                return BadRequest(new { message = $"{request.Name} does not exist", success = false });
            }

            var isMatch = _authService.VerifyPassword(request.Password, department.Password);

            if (!isMatch)
            {
                return NotFound(new { message = "Invalid Credentials", success = false });
            }

            var token = _authService.GenerateJwtToken(department.Id!, department.Name);

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login failed: {ex.Message}");
            return StatusCode(500, new { message = ex.Message, success = false });
        }
    }
}
