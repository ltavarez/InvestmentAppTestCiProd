using FluentAssertions;
using InvestmentApp.Core.Application.Features.AssetsHistories.Commands.CreateAssetHistory;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.AssetsHistories
{
    public class CreateAssetHistoryCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public CreateAssetHistoryCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"TestDb_AssetHistory_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_Return_Id_When_History_Is_Created()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var asset = new Core.Domain.Entities.Asset { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var repository = new AssetHistoryRepository(context);
            var handler = new CreateAssetHistoryCommandHandler(repository);

            var command = new CreateAssetHistoryCommand
            {
                AssetId = asset.Id,
                Value = 45320.75m,
                HistoryValueDate = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);

            var history = await context.AssetHistories.FindAsync(result);
            history.Should().NotBeNull();
            history!.Value.Should().Be(command.Value);
            history.AssetId.Should().Be(command.AssetId);
            history.HistoryValueDate.Should().Be(command.HistoryValueDate);
        }

        [Fact]
        public async Task Handle_Should_Return_Zero_When_AssetId_Does_Not_Exist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var repository = new AssetHistoryRepository(context);
            var handler = new CreateAssetHistoryCommandHandler(repository);

            var command = new CreateAssetHistoryCommand
            {
                AssetId = 999, // Asset no existe
                Value = 1000m,
                HistoryValueDate = DateTime.UtcNow
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0); 
        }
    }
}
