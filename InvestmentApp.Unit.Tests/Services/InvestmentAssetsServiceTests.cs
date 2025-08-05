using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Application.Services;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Services
{
    public class InvestmentAssetsServiceTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public InvestmentAssetsServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"TestDb_InvestmentAssetsService_{Guid.NewGuid()}")
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

        private InvestmentAssetsService CreateService()
        {
            var context = new InvestmentAppContext(_dbOptions);
            var repo = new InvestmentAssetRepository(context);
            return new InvestmentAssetsService(repo, _mapper);
        }

        [Fact]
        public async Task GetByAssetAndPortfolioAsync_Should_Return_Asset()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var userId = "user1";

            var asset = new Asset { Id = 0, Symbol = "USD", Name = "Dolar", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, Name = "Mi portafolio", UserId = userId };
            var investment = new InvestmentAssets { Id = 0, Asset = asset, AssetId = asset.Id, InvestmentPortfolio = portfolio, InvestmentPortfolioId = portfolio.Id };

            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            context.InvestmentAssets.Add(investment);
            await context.SaveChangesAsync();

            var service = CreateService();

            // Act
            var result = await service.GetByAssetAndPortfolioAsync(asset.Id, portfolio.Id, userId);

            // Assert
            result.Should().NotBeNull();
            result!.AssetId.Should().Be(asset.Id);
            result.InvestmentPortfolioId.Should().Be(portfolio.Id);
        }

        [Fact]
        public async Task GetByAssetAndPortfolioAsync_Should_Return_Null_If_Not_Found()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.GetByAssetAndPortfolioAsync(99, 99, "unknownUser");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_Should_Return_Item_When_UserId_Matches()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var userId = "user123";

            var asset = new Asset { Id = 0, Symbol = "ETH", Name = "Ethereum", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, Name = "Crypto Portfolio", UserId = userId };
            var investment = new InvestmentAssets { Id = 0, Asset = asset, AssetId = asset.Id, InvestmentPortfolio = portfolio, InvestmentPortfolioId = portfolio.Id };

            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            context.InvestmentAssets.Add(investment);
            await context.SaveChangesAsync();

            var service = CreateService();

            // Act
            var result = await service.GetById(investment.Id, userId);

            // Assert
            result.Should().NotBeNull();
            result!.AssetId.Should().Be(asset.Id);
            result.InvestmentPortfolio!.UserId.Should().Be(userId);
        }

        [Fact]
        public async Task GetById_Should_Return_Null_If_UserId_Does_Not_Match()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);

            var asset = new Asset { Id = 0, Symbol = "BTC", Name = "Bitcoin", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, Name = "Hidden Portfolio", UserId = "realUser" };
            var investment = new InvestmentAssets { Id = 0, Asset = asset, AssetId = asset.Id, InvestmentPortfolio = portfolio, InvestmentPortfolioId = portfolio.Id };

            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            context.InvestmentAssets.Add(investment);
            await context.SaveChangesAsync();

            var service = CreateService();

            // Act
            var result = await service.GetById(investment.Id, "fakeUser");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllWithInclude_Should_Return_User_Data_Only()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);

            var assetType = new AssetType { Id = 0, Name = "Equity" };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var asset = new Asset { Id = 0, Symbol = "XAU", Name = "Gold", AssetTypeId = assetType.Id };
            var portfolio1 = new InvestmentPortfolio { Id = 0, Name = "My Portfolio", UserId = "userA" };
            var portfolio2 = new InvestmentPortfolio { Id = 0, Name = "Other Portfolio", UserId = "userB" };

            var investment1 = new InvestmentAssets { Id = 0, Asset = asset, AssetId = asset.Id, InvestmentPortfolio = portfolio1, InvestmentPortfolioId = portfolio1.Id };
            var investment2 = new InvestmentAssets { Id = 0, Asset = asset, AssetId = asset.Id, InvestmentPortfolio = portfolio2, InvestmentPortfolioId = portfolio2.Id };

            context.Assets.Add(asset);
            context.InvestmentPortfolios.AddRange(portfolio1, portfolio2);
            context.InvestmentAssets.AddRange(investment1, investment2);
            await context.SaveChangesAsync();

            var service = CreateService();

            // Act
            var result = await service.GetAllWithInclude("userA");

            // Assert
            result.Should().HaveCount(1);
            result[0].InvestmentPortfolio!.UserId.Should().Be("userA");
        }

        [Fact]
        public async Task GetAllWithInclude_Should_Return_Empty_List_When_No_Match()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.GetAllWithInclude("unknown_user");

            // Assert
            result.Should().BeEmpty();
        }
    }
}
