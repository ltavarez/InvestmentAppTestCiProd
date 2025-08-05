using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Features.Assets.Queries.GetAllAssetsByPortfolioId;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Domain.Common.Enums;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.Asset
{
    public class GetAllAssetsByPortfolioIdQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetAllAssetsByPortfolioIdQueryHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"Db_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AssetMappingProfile>();
                cfg.AddProfile<AssetTypeMappingProfile>();
                cfg.AddProfile<AssetHistoryMappingProfile>();
                cfg.AddProfile<InvestmentAssetsMappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_Should_Return_Assets_Filtered_By_Portfolio()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var type = new Core.Domain.Entities.AssetType { Id = 1, Name = "Crypto" };
            context.AssetTypes.Add(type);

            var asset = new Core.Domain.Entities.Asset
            {
                Id = 1,
                Name = "Bitcoin",
                Symbol = "BTC",
                AssetTypeId = type.Id,
                AssetHistories =
                [
                    new() { Id = 1, Value = 50000m, HistoryValueDate = DateTime.UtcNow,AssetId = 1 }
                ]
            };

            var portfolio = new InvestmentPortfolio { Id = 1, UserId = "user1", Name = "Test Portfolio" };

            var relation = new Core.Domain.Entities.InvestmentAssets { Id = 1, AssetId = asset.Id, InvestmentPortfolioId = portfolio.Id };

            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            context.InvestmentAssets.Add(relation);

            await context.SaveChangesAsync();

            var handler = new GetAllAssetsByPortfolioIdQueryHandler(
                new AssetRepository(context),
                _mapper,
                new InvestmentAssetRepository(context));

            var query = new GetAllAssetsByPortfolioIdQuery { PortfolioId = portfolio.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Bitcoin");
            result[0].CurrentValue.Should().Be(50000m);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_When_Portfolio_Has_No_Assets()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var portfolio = new InvestmentPortfolio { Id = 2, UserId = "user2", Name = "Empty Portfolio" };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var handler = new GetAllAssetsByPortfolioIdQueryHandler(
                new AssetRepository(context),
                _mapper,
                new InvestmentAssetRepository(context));

            var query = new GetAllAssetsByPortfolioIdQuery { PortfolioId = portfolio.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_Should_Apply_AssetName_And_AssetTypeId_Filters()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var crypto = new Core.Domain.Entities.AssetType { Id = 1, Name = "Crypto" };
            var fiat = new Core.Domain.Entities.AssetType { Id = 2, Name = "Fiat" };
            context.AssetTypes.AddRange(crypto, fiat);

            var asset1 = new Core.Domain.Entities.Asset { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = crypto.Id, AssetHistories = [new() { Value = 100, HistoryValueDate = DateTime.UtcNow, Id = 0, AssetId = 1 }] };
            var asset2 = new Core.Domain.Entities.Asset { Id = 2, Name = "Dollar", Symbol = "USD", AssetTypeId = fiat.Id, AssetHistories = [new() { Value = 300, HistoryValueDate = DateTime.UtcNow, Id = 0, AssetId = 2 }] };
            context.Assets.AddRange(asset1, asset2);

            var portfolio = new InvestmentPortfolio { Id = 3, UserId = "user3", Name = "FilterTest" };
            context.InvestmentPortfolios.Add(portfolio);

            context.InvestmentAssets.AddRange(
                new Core.Domain.Entities.InvestmentAssets { AssetId = asset1.Id, InvestmentPortfolioId = portfolio.Id, Id = 0 },
                new Core.Domain.Entities.InvestmentAssets { AssetId = asset2.Id, InvestmentPortfolioId = portfolio.Id, Id = 0 });

            await context.SaveChangesAsync();

            var handler = new GetAllAssetsByPortfolioIdQueryHandler(
                new AssetRepository(context),
                _mapper,
                new InvestmentAssetRepository(context));

            var query = new GetAllAssetsByPortfolioIdQuery
            {
                PortfolioId = portfolio.Id,
                AssetName = "Bit",
                AssetTypeId = crypto.Id
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Bitcoin");
        }

        [Fact]
        public async Task Handle_Should_Order_By_CurrentValue_When_Requested()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var type = new Core.Domain.Entities.AssetType { Id = 1, Name = "Ordenables" };
            context.AssetTypes.Add(type);

            var asset1 = new Core.Domain.Entities.Asset
            {
                Id = 1,
                Name = "A",
                Symbol = "A",
                AssetTypeId = 1,
                AssetHistories = [new() { Value = 100, HistoryValueDate = DateTime.UtcNow, Id = 0, AssetId = 1 }]
            };

            var asset2 = new Core.Domain.Entities.Asset
            {
                Id = 2,
                Name = "B",
                Symbol = "B",
                AssetTypeId = 1,
                AssetHistories = [new() { Value = 300, HistoryValueDate = DateTime.UtcNow, Id = 0, AssetId = 2 }]
            };

            var portfolio = new InvestmentPortfolio { Id = 4, UserId = "user4", Name = "Ordered Portfolio" };

            context.Assets.AddRange(asset1, asset2);
            context.InvestmentPortfolios.Add(portfolio);
            context.InvestmentAssets.AddRange(
                new Core.Domain.Entities.InvestmentAssets { AssetId = 1, InvestmentPortfolioId = 4, Id = 0 },
                new Core.Domain.Entities.InvestmentAssets { AssetId = 2, InvestmentPortfolioId = 4, Id = 0 });

            await context.SaveChangesAsync();

            var handler = new GetAllAssetsByPortfolioIdQueryHandler(
                new AssetRepository(context),
                _mapper,
                new InvestmentAssetRepository(context));

            var query = new GetAllAssetsByPortfolioIdQuery
            {
                PortfolioId = 4,
                AssetOrderBy = (int)AssetOrdered.BY_CURRENT_VALUE
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result[0].CurrentValue.Should().Be(300); // orden descendente
            result[1].CurrentValue.Should().Be(100);
        }
    }
}
