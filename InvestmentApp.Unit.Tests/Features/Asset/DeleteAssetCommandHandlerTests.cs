using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.Assets.Commands.DeleteAsset;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.Asset
{
    public class DeleteAssetCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public DeleteAssetCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"TestDb_DeleteAsset_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_Delete_Asset_When_It_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Core.Domain.Entities.Asset { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var repository = new AssetRepository(context);
            var handler = new DeleteAssetCommandHandler(repository);

            var command = new DeleteAssetCommand { Id = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var deleted = await context.Assets.FindAsync(1);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Asset_Does_Not_Exist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);
            var handler = new DeleteAssetCommandHandler(repository);

            var command = new DeleteAssetCommand { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Asset not found with this id");
        }
    }
}

