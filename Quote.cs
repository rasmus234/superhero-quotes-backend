using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LocalFunctionProj;

public class Quote
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("text")]
    public string Text { get; set; }
    
    [BsonElement("author")]
    public string Author { get; set; }
}