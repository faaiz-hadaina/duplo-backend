using MongoDB.Driver;
using DuploBackend.Models;
using DuploBackend.Data;

namespace DuploBackend.Utils;

public class GenerateRandomUtils
{
    private readonly MongoDbContext _context;
    private readonly Random _random = new();

    public GenerateRandomUtils(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<int> GenerateUniqueBusinessID()
    {
        while (true)
        {
            var businessID = _random.Next(1000, 10000);
            var existingDepartment = await _context.Departments
                .Find(d => d.BusinessID == businessID)
                .FirstOrDefaultAsync();

            if (existingDepartment == null)
            {
                return businessID;
            }
        }
    }
}
