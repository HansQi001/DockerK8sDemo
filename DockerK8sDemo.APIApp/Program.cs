using Azure;
using Azure.Core;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text.Json;
using System.Web;
using static Azure.Core.HttpHeader;

namespace DockerK8sDemo.APIApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register CosmosClient as a singleton
            builder.Services.AddSingleton(sp =>
            {
                var connectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");
                return new CosmosClient(connectionString, new CosmosClientOptions { AllowBulkExecution = true });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/api/products", async (HttpContext httpContext
                , CosmosClient cosmosClient) =>
            {
                httpContext.Response.Headers["Content-Type"] = "application/x-ndjson";

                var container = cosmosClient
                                .GetDatabase("ServerlessDemo")
                                .GetContainer("Products");

                await using var writer = new StreamWriter(httpContext.Response.Body);
                using var feedIterator = container.GetItemQueryIterator<Product>(
                    $"SELECT * FROM c");

                while (feedIterator.HasMoreResults)
                {
                    var page = await feedIterator.ReadNextAsync();
                    foreach (var product in page)
                    {
                        //var dto = _mapper.Map(product).ToANew<ProductSummaryDTO>();
                        // Serialize each product as JSON and write immediately
                        var json = System.Text.Json.JsonSerializer.Serialize(product, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                        await writer.WriteLineAsync(json);
                        // push chunk to client
                        await writer.FlushAsync();
                    }
                }
            })
            .WithName("GetAllProducts")
            .WithOpenApi();

            app.MapGet("/api/products/{id}", async (string id, CosmosClient cosmosClient) =>
            {
                var container = cosmosClient.GetDatabase("ServerlessDemo").GetContainer("Products");

                try
                {
                    var response = await container.ReadItemAsync<Product>(id, new PartitionKey(id));
                    return Results.Ok(response.Resource);
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return Results.NotFound();
                }

            })
            .WithName("ProductDetails")
            .WithOpenApi(); ;

            app.MapGet("/api/version", () => Results.Ok("1.0")).WithName("APIVersion")
            .WithOpenApi(); ;

            app.Run();
        }
    }

    public class Product
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("stock")]
        public int Stock { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; } = "Active";

        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [JsonProperty("lastModifiedAt")]
        public DateTimeOffset? LastModifiedAt { get; set; }
    }
}
