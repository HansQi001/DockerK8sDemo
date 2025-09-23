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

            app.MapGet("/api/products", ProductsEndpoints.GetAllProductsAync)
            .WithName("GetAllProducts")
            .WithOpenApi();

            app.MapGet("/api/products/{id}", ProductsEndpoints.GetProductAsync)
            .WithName("ProductDetails")
            .WithOpenApi();

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
