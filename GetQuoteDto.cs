using System.Text.Json.Serialization;

namespace LocalFunctionProj;

public class GetQuoteDto
{
    [JsonPropertyName("author")]
    public string Author { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; }
}