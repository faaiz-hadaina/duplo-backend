using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DuploBackend.Models;

public class Department
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("password")]
    public string Password { get; set; } = string.Empty;

    [BsonElement("businessID")]
    public int BusinessID { get; set; }
}
