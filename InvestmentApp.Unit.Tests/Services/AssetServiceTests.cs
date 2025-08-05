using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Application.Services;
using InvestmentApp.Core.Domain.Common.Enums;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Services
{
    public class AssetServiceTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public AssetServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"TestDb_AssetService_{Guid.NewGuid()}")
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

        private AssetService CreateService()
        {
            var context = new InvestmentAppContext(_dbOptions);
            var assetRepo = new AssetRepository(context);
            var investmentAssetRepo = new InvestmentAssetRepository(context);
            return new AssetService(assetRepo, investmentAssetRepo, _mapper);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Asset()
        {
            //Arrange
            var service = CreateService();
            var context = new InvestmentAppContext(_dbOptions);
            var assetType = new AssetType { Id = 0, Name = "Crypto" };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();
            var dto = new AssetDto { Id = 0, Symbol = "BTC", AssetTypeId = assetType.Id, Name = "Bitcoin" };

            //Act
            var result = await service.AddAsync(dto);

            //Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetById_Should_Return_Asset_With_AssetType()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var assetType = new AssetType { Id = 0, Name = "Equity" };
            var asset = new Asset { Id = 0, Name = "Bonos", AssetTypeId = assetType.Id, Symbol = "AAPL", AssetType = assetType };
            context.AssetTypes.Add(assetType);
            context.Assets.Add(asset);
            await context.SaveChangesAsync();
            var service = CreateService();

            // Act
            var result = await service.GetById(asset.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Symbol.Should().Be("AAPL");
            result.AssetType.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Symbol()
        {
            // Arrange
            var service = CreateService();
            var context = new InvestmentAppContext(_dbOptions);
            var assetType = new AssetType { Id = 0, Name = "Forex" };
            var asset = new Asset { Id = 0, Name = "Dolar", Symbol = "USD", AssetType = assetType, AssetTypeId = assetType.Id };
            context.AssetTypes.Add(assetType);
            context.Assets.Add(asset);
            await context.SaveChangesAsync();
            var dto = new AssetDto { Id = asset.Id, Symbol = "USDX", AssetTypeId = assetType.Id, Name = "Dolar digital" };

            // Act
            var updated = await service.UpdateAsync(dto, asset.Id);

            // Assert
            updated.Should().NotBeNull();
            updated!.Symbol.Should().Be("USDX");
        }

        [Fact]
        public async Task DeleteAsync_Should_Delete_Asset()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var assetType = new AssetType { Id = 0, Name = "Gold" };
            var asset = new Asset { Id = 0, Symbol = "XAU", AssetType = assetType, AssetTypeId = assetType.Id, Name = "Cripto" };
            context.AssetTypes.Add(assetType);
            context.Assets.Add(asset);
            await context.SaveChangesAsync();
            var service = CreateService();

            // Act
            var result = await service.DeleteAsync(asset.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAll_Should_Return_List()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var assetType = new AssetType { Id = 0, Name = "Crypto" };
            context.AssetTypes.Add(assetType);
            context.Assets.AddRange(
                new Asset { Id = 0, Symbol = "BTC", AssetType = assetType, AssetTypeId = assetType.Id, Name = "Bitcoind" },
                new Asset { Id = 0, Symbol = "ETH", AssetType = assetType, AssetTypeId = assetType.Id, Name = "Ethereum" });
            await context.SaveChangesAsync();
            var service = CreateService();

            // Act
            var result = await service.GetAll();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllWithInclude_Should_Return_Assets_With_AssetType_And_Histories()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var assetType = new AssetType { Id = 0, Name = "Equity" };
            var asset = new Asset
            {
                Id = 0,
                Symbol = "MSFT",
                AssetType = assetType,
                AssetTypeId = assetType.Id,
                Name = "Microsoft",
                AssetHistories =
                [
                    new AssetHistory { AssetId = 0,Id = 0,HistoryValueDate = DateTime.UtcNow, Value = 350 }
                ]
            };
            context.AssetTypes.Add(assetType);
            context.Assets.Add(asset);
            await context.SaveChangesAsync();
            var service = CreateService();

            // Act
            var result = await service.GetAllWithInclude();

            // Assert
            result.Should().HaveCount(1);
            result[0].AssetHistories.Should().NotBeNull();
            result[0].AssetHistories.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllAssetsByPortfolioId_Should_Return_Filtered_Data()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
     
            var assetType = new AssetType { Id = 0, Name = "Forex" };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var asset1 = new Asset { Id = 0, Symbol = "USD", Name = "Dolar", AssetTypeId = assetType.Id };
            var asset2 = new Asset { Id = 0, Symbol = "EUR", Name = "Euro", AssetTypeId = assetType.Id };
            context.Assets.AddRange(asset1, asset2);
            await context.SaveChangesAsync();
    
            context.AssetHistories.Add(new AssetHistory
            {
                Id = 0,
                AssetId = asset1.Id,
                Value = 1.05m,
                HistoryValueDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
         
            var portfolio = new InvestmentPortfolio { Id = 0, UserId = "user1", Name = "Test" };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            context.InvestmentAssets.Add(new InvestmentAssets
            {
                Id = 0,
                AssetId = asset1.Id,
                InvestmentPortfolioId = portfolio.Id
            });
            await context.SaveChangesAsync();    
            var service = CreateService();

            // Act
            var result = await service.GetAllAssetsByPortfolioId(portfolio.Id);

            // Assert
            result.Should().HaveCount(1);
            result[0].Symbol.Should().Be("USD");
            result[0].CurrentValue.Should().Be(1.05m);
        }

        [Fact]
        public async Task GetAllAssetsByPortfolioId_Should_Return_Empty_When_No_Assets()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var portfolio = new InvestmentPortfolio { Id = 0, UserId = "noAssets", Name = "Test" };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();
            var service = CreateService();

            // Act
            var result = await service.GetAllAssetsByPortfolioId(portfolio.Id);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllAssetsByPortfolioId_Should_Apply_Filters_And_Order()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
        
            var assetType = new AssetType { Id = 0, Name = "Equity" };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var assetA = new Asset { Id = 0, Symbol = "AAPL", Name = "AAPL", AssetTypeId = assetType.Id };
            var assetB = new Asset { Id = 0, Symbol = "TSLA", Name = "TSLA", AssetTypeId = assetType.Id };
            context.Assets.AddRange(assetA, assetB);
            await context.SaveChangesAsync();

            context.AssetHistories.AddRange(
                new AssetHistory { Id = 0, AssetId = assetA.Id, Value = 100m, HistoryValueDate = DateTime.UtcNow.AddDays(-1) },
                new AssetHistory { Id = 0, AssetId = assetA.Id, Value = 120m, HistoryValueDate = DateTime.UtcNow }, // último valor
                new AssetHistory { Id = 0, AssetId = assetB.Id, Value = 300m, HistoryValueDate = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            var portfolio = new InvestmentPortfolio { Id = 0, Name = "Test", UserId = "user2" };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();
            
            context.InvestmentAssets.AddRange(
                new InvestmentAssets { Id = 0, AssetId = assetA.Id, InvestmentPortfolioId = portfolio.Id },
                new InvestmentAssets { Id = 0, AssetId = assetB.Id, InvestmentPortfolioId = portfolio.Id });
            await context.SaveChangesAsync();            
            var service = CreateService();

            // Act
            var result = await service.GetAllAssetsByPortfolioId(
                portfolio.Id, assetName: "AAPL", assetTypeId: assetType.Id, assetOrderBy: (int)AssetOrdered.BY_NAME);

            // Assert
            result.Should().HaveCount(1);
            result[0].Symbol.Should().Be("AAPL");
            result[0].CurrentValue.Should().Be(120m); // último valor de AAPL
        }
    }
}