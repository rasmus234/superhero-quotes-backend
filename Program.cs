using LocalFunctionProj;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .Build();

//read the quotes.json file and deserialize it to a list of Quote objects with system.text.json
var quoteDtos = JsonSerializer.Deserialize<List<GetQuoteDto>>(File.ReadAllText("quotes.json"));
var quotes = quoteDtos.Select(quoteDto => new Quote
{
    Id = new ObjectId(),
    Author = quoteDto.Author,
    Text = quoteDto.Text
});

//connect to MongoDB
var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
var client = new MongoClient(connectionString);
var database = client.GetDatabase("QuoteDB");
var quoteCollection = database.GetCollection<Quote>("quotes");
//Delete all quotes from the database
await quoteCollection.DeleteManyAsync(FilterDefinition<Quote>.Empty);

//insert the quotes into the database
await quoteCollection.InsertManyAsync(quotes);


host.Run();
