using DockerK8sDemo.APIApp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Cosmos;
using Moq;

namespace DockerK8sDemo.APIAppTests
{
    public class ProductsTests
    {
        [Fact]
        public async Task WithoutId_Should_Returns_Ok()
        {
            // Arrange
            var productId = "123";
            var expectedProduct = new Product { Id = productId, Name = "Test Product" };

            var mockResponse = new Mock<ItemResponse<Product>>();
            mockResponse.Setup(r => r.Resource).Returns(expectedProduct);

            var mockContainer = new Mock<Container>();
            mockContainer
                .Setup(c => c.ReadItemAsync<Product>(productId, It.IsAny<PartitionKey>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse.Object);

            var mockDatabase = new Mock<Database>();
            mockDatabase.Setup(d => d.GetContainer("Products")).Returns(mockContainer.Object);

            var mockClient = new Mock<CosmosClient>();
            mockClient.Setup(c => c.GetDatabase("ServerlessDemo")).Returns(mockDatabase.Object);

            // Act
            var result = await ProductsEndpoints.GetProductAsync(productId, mockClient.Object);

            // Assert
            var okResult = Assert.IsType<Ok<Product>>(result);
            Assert.Equal(expectedProduct, okResult.Value);

        }
    }
}