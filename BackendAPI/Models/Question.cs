using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BackendAPI.Models;

public class Question
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int QuestionTypeId { get; set; }
    public string ResponseCategory { get; set; } = string.Empty;
    public ResponseOptions ResponseOptions { get; set; } = new();
}

public class ResponseOptions
{
    public List<string> Choices { get; set; } = new();
    public string? Answer { get; set; }
}
