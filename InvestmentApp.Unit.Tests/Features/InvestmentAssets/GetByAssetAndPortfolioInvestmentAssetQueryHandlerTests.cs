using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetByAssetAndPortfolio;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentAssets
{
    public class GetByAssetAndPortfolioInvestmentAssetQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetByAssetAndPortfolioInvestmentAssetQueryHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"Db_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AssetMappingProfile>();
                cfg.AddProfile<AssetTypeMappingProfile>();
                cfg.AddProfile<AssetHistoryMappingProfile>();
                cfg.AddProfile<AssetTypeMappingProfile>();
                cfg.AddProfile<InvestmentPortFolioMappingProfile>();
                cfg.AddProfile<InvestmentAssetsMappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_Should_Return_InvestmentAsset_When_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var userId = "user-1";
            var assetType = new Core.Domain.Entities.AssetType { Id = 0, Name = "Equity" };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var asset = new Core.Domain.Entities.Asset { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = assetType.Id };
            var portfolio = new InvestmentPortfolio { Id = 1, UserId = userId, Name = "Main Portfolio" };

            var investmentAsset = new Core.Domain.Entities.InvestmentAssets
            {
                Id = 1,
                AssetId = asset.Id,
                InvestmentPortfolioId = portfolio.Id,
                Asset = asset,
                InvestmentPortfolio = portfolio,
                AssociationDate = DateTime.UtcNow
            };

            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            context.InvestmentAssets.Add(investmentAsset);
            await context.SaveChangesAsync();

            var repository = new InvestmentAssetRepository(context);
            var handler = new GetByAssetAndPortfolioInvestmentAssetQueryHandler(repository, _mapper);

            var query = new GetByAssetAndPortfolioInvestmentAssetQuery
            {
                AssetId = asset.Id,
                PortfolioId = portfolio.Id,
                UserId = userId
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AssetId.Should().Be(asset.Id);
            result.InvestmentPortfolioId.Should().Be(portfolio.Id);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_NotFound()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var repository = new InvestmentAssetRepository(context);
            var handler = new GetByAssetAndPortfolioInvestmentAssetQueryHandler(repository, _mapper);

            var query = new GetByAssetAndPortfolioInvestmentAssetQuery
            {
                AssetId = 99,
                PortfolioId = 99,
                UserId = "nonexistent-user"
            };

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Investment Assets not found with this id");
        }
    }
}
