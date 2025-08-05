using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.AssetsHistories.Commands.UpdateAssetHistory;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.AssetsHistories
{
    public class UpdateAssetHistoryCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public UpdateAssetHistoryCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_UpdateAssetHistory_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_Update_AssetHistory_When_Id_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var initial = new AssetHistory
            {
                Id = 1,
                AssetId = 5,
                Value = 2000m,
                HistoryValueDate = new DateTime(2024, 12, 30, 0, 0, 0, DateTimeKind.Utc)
            };

            context.AssetHistories.Add(initial);
            await context.SaveChangesAsync();

            var repository = new AssetHistoryRepository(context);
            var handler = new UpdateAssetHistoryCommandHandler(repository);

            var command = new UpdateAssetHistoryCommand
            {
                Id = initial.Id,        
                HistoryValueDate = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                AssetId = 10,
                Value = 3000m             
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);

            var updated = await context.AssetHistories.FindAsync(command.Id);
            updated.Should().NotBeNull();
            updated!.AssetId.Should().Be(command.AssetId);
            updated.Value.Should().Be(command.Value);
            updated.HistoryValueDate.Should().Be(command.HistoryValueDate);
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var repository = new AssetHistoryRepository(context);
            var handler = new UpdateAssetHistoryCommandHandler(repository);

            var command = new UpdateAssetHistoryCommand
            {
                Id = 999,
                AssetId = 1,
                Value = 1500m,
                HistoryValueDate = DateTime.UtcNow
            };

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Asset history not found with this id");
        }
    }
}
