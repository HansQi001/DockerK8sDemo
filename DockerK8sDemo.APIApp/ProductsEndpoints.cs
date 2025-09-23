using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace DockerK8sDemo.APIApp
{
    public static class ProductsEndpoints
    {
        public static async Task<IResult> GetProductAsync(string id, CosmosClient cosmosClient)
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
        }

        public static async Task GetAllProductsAync(HttpContext httpContext, CosmosClient cosmosClient)
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
                    var json = JsonSerializer.Serialize(product, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    await writer.WriteLineAsync(json);
                    // push chunk to client
                    await writer.FlushAsync();
                }
            }
        }
    }
}
