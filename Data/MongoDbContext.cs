using MongoDB.Driver;
using DuploBackend.Models;
using Microsoft.Extensions.Options;

namespace DuploBackend.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<Business> Businesses => 
        _database.GetCollection<Business>("businesses");

    public IMongoCollection<Department> Departments => 
        _database.GetCollection<Department>("departments");

    public IMongoCollection<Transaction> Transactions => 
        _database.GetCollection<Transaction>("transactions");
}
