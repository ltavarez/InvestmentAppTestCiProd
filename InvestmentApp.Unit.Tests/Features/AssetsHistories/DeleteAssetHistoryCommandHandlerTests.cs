using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.AssetsHistories.Commands.DeleteAssetHistory;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.AssetsHistories
{
    public class DeleteAssetHistoryCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public DeleteAssetHistoryCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_DeleteAssetHistory_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_Delete_AssetHistory_When_Id_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var assetHistory = new AssetHistory
            {
                Id = 1,
                AssetId = 1,
                Value = 1000m,
                HistoryValueDate = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc)
            };

            context.AssetHistories.Add(assetHistory);         
            await context.SaveChangesAsync();

            var repository = new AssetHistoryRepository(context);
            var handler = new DeleteAssetHistoryCommandHandler(repository);

            var command = new DeleteAssetHistoryCommand { Id = assetHistory.Id };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var deleted = await context.AssetHistories.FindAsync(assetHistory.Id);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Id_Not_Found()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetHistoryRepository(context);
            var handler = new DeleteAssetHistoryCommandHandler(repository);

            var command = new DeleteAssetHistoryCommand { Id = 999 };

            // Act
            var act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Asset history not found with this id");
        }
    }
}
