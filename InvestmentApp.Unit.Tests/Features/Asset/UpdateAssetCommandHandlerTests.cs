using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.Assets.Commands.UpdateAsset;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.Asset
{
    public class UpdateAssetCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public UpdateAssetCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"TestDb_UpdateAsset_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_Update_Asset_When_It_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var existingAsset = new Core.Domain.Entities.Asset
            {
                Id = 1,
                Name = "Bitcoin",
                Symbol = "BTC",
                Description = "Cripto",
                AssetTypeId = 1
            };

            context.Assets.Add(existingAsset);
            await context.SaveChangesAsync();

            var repository = new AssetRepository(context);
            var handler = new UpdateAssetCommandHandler(repository);

            var command = new UpdateAssetCommand
            {
                Id = 1,
                Name = "Bitcoin Updated",
                Symbol = "BTCX",
                Description = "Criptomoneda actualizada",
                AssetTypeId = 2
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);

            var updated = await context.Assets.FindAsync(1);
            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Bitcoin Updated");
            updated.Symbol.Should().Be("BTCX");
            updated.Description.Should().Be("Criptomoneda actualizada");
            updated.AssetTypeId.Should().Be(2);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Asset_Does_Not_Exist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);
            var handler = new UpdateAssetCommandHandler(repository);

            var command = new UpdateAssetCommand
            {
                Id = 999,
                Name = "No existe",
                Symbol = "NONE",
                Description = "No hay",
                AssetTypeId = 1
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Asset not found with this id");
        }
    }
}
