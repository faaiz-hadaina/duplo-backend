using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DuploBackend.Models;

public class Transaction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("businessID")]
    public int BusinessID { get; set; }

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;
}
