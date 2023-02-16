using System.Net;
using System.Text.Json;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LocalFunctionProj
{
    public class QuoteFunctions
    {
        private readonly ILogger _logger;

        public QuoteFunctions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<QuoteFunctions>();
        }

        [Function("quote")]
        public async Task<HttpResponseData> GetQuote(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]
            HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                //connect to MongoDB
                var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
                var client = new MongoClient(connectionString);
                _logger.LogInformation("Connected to MongoDB");
                var database = client.GetDatabase("QuoteDB");
                var quoteCollection = database.GetCollection<Quote>("quotes");

                //get random quote
                var collectionCount = await quoteCollection.CountDocumentsAsync(FilterDefinition<Quote>.Empty);
                var randomQuote = await quoteCollection
                    .Find(FilterDefinition<Quote>.Empty)
                    .Skip((int)(collectionCount * new Random().NextDouble()))
                    .Limit(1)
                    .FirstOrDefaultAsync();

                //create response, serialize quote to JSON as a GetQuoteDto and return the response
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new GetQuoteDto
                {
                    Author = randomQuote.Author,
                    Text = randomQuote.Text
                });
                _logger.LogInformation("Returning quote");
                return response;
            }
            catch (Exception e)
            {
                //log error and return 500
                _logger.LogError(e, "Error");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
            
        }
    }
}