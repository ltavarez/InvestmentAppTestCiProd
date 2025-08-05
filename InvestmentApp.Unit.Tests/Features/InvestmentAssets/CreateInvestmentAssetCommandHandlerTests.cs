using FluentAssertions;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Commands.CreateInvestmentAsset;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentAssets
{
    public class CreateInvestmentAssetCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public CreateInvestmentAssetCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_InvestAsset_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldReturnId_WhenCreationIsSuccessful()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
        
            var asset = new Core.Domain.Entities.Asset { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 1, UserId = "user1", Name = "My Portfolio" };

            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var repository = new InvestmentAssetRepository(context);
            var handler = new CreateInvestmentAssetCommandHandler(repository);

            var command = new CreateInvestmentAssetCommand
            {
                AssetId = asset.Id,
                InvestmentPortfolioId = portfolio.Id,
                AssociationDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);

            var created = await context.InvestmentAssets.FindAsync(result);
            created.Should().NotBeNull();
            created!.AssetId.Should().Be(command.AssetId);
            created.InvestmentPortfolioId.Should().Be(command.InvestmentPortfolioId);
            created.AssociationDate.Should().Be(command.AssociationDate);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenRepositoryReturnsNull()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var mockRepo = new FailingInvestmentAssetRepository(context); 
            var handler = new CreateInvestmentAssetCommandHandler(mockRepo);

            var command = new CreateInvestmentAssetCommand
            {
                AssetId = 99,
                InvestmentPortfolioId = 99
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(0);
        }
    
        private class FailingInvestmentAssetRepository(InvestmentAppContext context) : InvestmentAssetRepository(context)
        {
            public override Task<Core.Domain.Entities.InvestmentAssets?> AddAsync(Core.Domain.Entities.InvestmentAssets entity)
                => Task.FromResult<Core.Domain.Entities.InvestmentAssets?>(null);
        }
    }
}
