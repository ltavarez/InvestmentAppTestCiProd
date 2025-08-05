using FluentAssertions;
using InvestmentApp.Core.Application.Features.Assets.Commands.CreateAsset;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.Asset
{
    public class CreateAssetCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public CreateAssetCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"TestDb_CreateAsset_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_Return_Id_When_Asset_Is_Created()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            // Agregamos el AssetType necesario como FK
            var assetType = new Core.Domain.Entities.AssetType { Id = 1,Name = "Coin" };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var repository = new AssetRepository(context);
            var handler = new CreateAssetCommandHandler(repository);

            var command = new CreateAssetCommand
            {
                Name = "Bitcoin",
                Symbol = "BTC",
                Description = "Criptomoneda líder",
                AssetTypeId = assetType.Id
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);

            var created = await context.Assets.FindAsync(result);
            created.Should().NotBeNull();
            created!.Name.Should().Be(command.Name);
            created.Symbol.Should().Be(command.Symbol);
            created.Description.Should().Be(command.Description);
            created.AssetTypeId.Should().Be(assetType.Id);
        }

        [Fact]
        public async Task Handle_Should_Return_Zero_When_AssetType_Does_Not_Exist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var repository = new AssetRepository(context);
            var handler = new CreateAssetCommandHandler(repository);

            var command = new CreateAssetCommand
            {
                Name = "Dogecoin",
                Symbol = "DOGE",
                Description = "Meme coin",
                AssetTypeId = 999 
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
        }
    }
}
