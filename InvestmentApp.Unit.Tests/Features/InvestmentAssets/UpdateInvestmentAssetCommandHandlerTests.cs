using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.UpdateInvestmentAsset;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentAssets
{
    public class UpdateInvestmentAssetCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public UpdateInvestmentAssetCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_UpdateInvestAsset_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_Update_InvestmentAsset_When_ItExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var existing = new Core.Domain.Entities.InvestmentAssets
            {
                Id = 1,
                AssetId = 1,
                InvestmentPortfolioId = 1,
                AssociationDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc)
            };

            context.InvestmentAssets.Add(existing);
            await context.SaveChangesAsync();

            var repository = new InvestmentAssetRepository(context);
            var handler = new UpdateInvestmentAssetCommandHandler(repository);

            var command = new UpdateInvestmentAssetCommand
            {
                Id = 1,
                AssetId = 2,
                InvestmentPortfolioId = 3,
                AssociationDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var updated = await context.InvestmentAssets.FindAsync(1);
            updated!.AssetId.Should().Be(2);
            updated.InvestmentPortfolioId.Should().Be(3);
            updated.AssociationDate.Should().Be(command.AssociationDate);
        }

        [Fact]
        public async Task Handle_Should_Throw_ArgumentException_When_InvestmentAsset_NotFound()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var repository = new InvestmentAssetRepository(context);
            var handler = new UpdateInvestmentAssetCommandHandler(repository);

            var command = new UpdateInvestmentAssetCommand
            {
                Id = 999,
                AssetId = 1,
                InvestmentPortfolioId = 1,
                AssociationDate = DateTime.UtcNow
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Investment Assets not found with this id");
        }
    }
}
