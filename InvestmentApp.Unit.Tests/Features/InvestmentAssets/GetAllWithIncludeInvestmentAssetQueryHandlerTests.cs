using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetAllWithInclude;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentAssets
{
    public class GetAllWithIncludeInvestmentAssetQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetAllWithIncludeInvestmentAssetQueryHandlerTests()
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
        public async Task Handle_Should_Return_InvestmentAssets_For_User()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var userId = "user-1";

            var assetType = new Core.Domain.Entities.AssetType { Id = 0, Name = "Equity" };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var asset = new Core.Domain.Entities.Asset { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = assetType.Id };
            var portfolio = new InvestmentPortfolio { Id = 1, UserId = userId, Name = "My Portfolio" };
            var investment = new Core.Domain.Entities.InvestmentAssets
            {
                Id = 1,
                AssetId = asset.Id,
                Asset = asset,
                InvestmentPortfolioId = portfolio.Id,
                InvestmentPortfolio = portfolio,
                AssociationDate = new DateTime(2024, 01, 01,0,0,0,DateTimeKind.Utc)
            };

            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            context.InvestmentAssets.Add(investment);
            await context.SaveChangesAsync();

            var repository = new InvestmentAssetRepository(context);
            var handler = new GetAllWithIncludeInvestmentAssetQueryHandler(repository, _mapper);

            var query = new GetAllWithIncludeInvestmentAssetQuery { UserId = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result[0].AssetId.Should().Be(asset.Id);
            result[0].InvestmentPortfolioId.Should().Be(portfolio.Id);
            result[0].InvestmentPortfolio.Should().NotBeNull();
            result[0].Asset.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_Should_Return_EmptyList_When_NoData_For_User()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var repository = new InvestmentAssetRepository(context);
            var handler = new GetAllWithIncludeInvestmentAssetQueryHandler(repository, _mapper);

            var query = new GetAllWithIncludeInvestmentAssetQuery { UserId = "non-existent-user" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
