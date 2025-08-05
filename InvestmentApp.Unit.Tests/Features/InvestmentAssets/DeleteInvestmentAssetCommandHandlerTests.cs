using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.DeleteInvestmentAsset;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentAssets
{
    public class DeleteInvestmentAssetCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public DeleteInvestmentAssetCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_DeleteInvestAsset_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldDeleteInvestmentAsset_WhenItExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var investmentAsset = new Core.Domain.Entities.InvestmentAssets
            {
                Id = 1,
                AssetId = 1,
                InvestmentPortfolioId = 1,
                AssociationDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
            };

            context.InvestmentAssets.Add(investmentAsset);
            await context.SaveChangesAsync();

            var repository = new InvestmentAssetRepository(context);
            var handler = new DeleteInvestmentAssetCommandHandler(repository);

            var command = new DeleteInvestmentAssetCommand { Id = investmentAsset.Id };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var deleted = await context.InvestmentAssets.FindAsync(investmentAsset.Id);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenAssetDoesNotExist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new InvestmentAssetRepository(context);
            var handler = new DeleteInvestmentAssetCommandHandler(repository);

            var command = new DeleteInvestmentAssetCommand { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Investment Asset not found with this id");
        }
    }
}
