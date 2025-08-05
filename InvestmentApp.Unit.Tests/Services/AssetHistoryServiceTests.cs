using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Dtos.AssetHistory;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Application.Services;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Services
{
    public class AssetHistoryServiceTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public AssetHistoryServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"TestDb_AssetHistory_{Guid.NewGuid()}")
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

        private AssetHistoryService CreateService()
        {
            var context = new InvestmentAppContext(_dbOptions);
            var repo = new AssetHistoryRepository(context);
            return new AssetHistoryService(repo, _mapper);
        }

        [Fact]
        public async Task AddAsync_Should_Add_AssetHistory()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Symbol = "BTC", Name = "Bitcoin", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var service = CreateService();
            var dto = new AssetHistoryDto
            {
                AssetId = asset.Id,
                HistoryValueDate = DateTime.UtcNow,
                Value = 100m,
                Id = 0
            };

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().Be(100m);
        }

        [Fact]
        public async Task UpdateAsync_Should_Preserve_HistoryValueDate()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Symbol = "ETH", Name = "Ethereum", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var history = new AssetHistory
            {
                Id = 0,
                AssetId = asset.Id,
                Value = 1500m,
                HistoryValueDate = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            };
            context.AssetHistories.Add(history);
            await context.SaveChangesAsync();

            var service = CreateService();
            var dto = new AssetHistoryDto
            {
                Id = history.Id,
                AssetId = asset.Id,
                Value = 1800m,
                HistoryValueDate = DateTime.UtcNow
            };

            // Act
            var updated = await service.UpdateAsync(dto, history.Id);

            // Assert
            updated.Should().NotBeNull();
            updated!.Value.Should().Be(1800m);
            updated.HistoryValueDate.Should().Be(new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)); 
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_NotExists()
        {
            // Arrange
            var service = CreateService();

            var dto = new AssetHistoryDto
            {
                Id = 999,
                AssetId = 1,
                Value = 100,
                HistoryValueDate = DateTime.UtcNow
            };

            // Act
            var result = await service.UpdateAsync(dto, 999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_AssetHistory()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
           
            var asset = new Asset { Id = 0, Symbol = "XAU", Name = "Gold", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var history = new AssetHistory
            {
                Id = 0,
                AssetId = asset.Id,
                Value = 1800,
                HistoryValueDate = DateTime.UtcNow
            };
            context.AssetHistories.Add(history);
            await context.SaveChangesAsync();
            var repo = new AssetHistoryRepository(context);
            var service = new AssetHistoryService(repo, _mapper);

            //Act
            var result = await service.DeleteAsync(history.Id);

            // Assert
            result.Should().BeTrue();            
            var deleted = await context.AssetHistories.FindAsync(history.Id);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task GetById_Should_Return_History_With_Asset()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Symbol = "MSFT", Name = "Microsoft", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var history = new AssetHistory
            {
                Id = 0,
                AssetId = asset.Id,
                Value = 300m,
                HistoryValueDate = DateTime.UtcNow
            };
            context.AssetHistories.Add(history);
            await context.SaveChangesAsync();
            var service = CreateService();

            // Act
            var result = await service.GetById(history.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().Be(300m);
            result.Asset.Should().NotBeNull();
            result.Asset!.Symbol.Should().Be("MSFT");
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotFound()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.GetById(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_Should_Return_All_AssetHistories()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Symbol = "TSLA", Name = "Tesla", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            context.AssetHistories.AddRange(
                new AssetHistory { Id = 0, AssetId = asset.Id, Value = 800, HistoryValueDate = DateTime.UtcNow },
                new AssetHistory { Id = 0, AssetId = asset.Id, Value = 850, HistoryValueDate = DateTime.UtcNow.AddDays(-1) }
            );
            await context.SaveChangesAsync();
            var service = CreateService();

            // Act
            var result = await service.GetAll();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllWithInclude_Should_Return_With_Asset()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            
            var assetType = new AssetType { Id = 0, Name = "Equity" };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var asset = new Asset { Id = 0, Symbol = "GOOGL", Name = "Google", AssetTypeId = assetType.Id };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            context.AssetHistories.Add(new AssetHistory
            {
                Id = 0,
                AssetId = asset.Id,
                Value = 2800,
                HistoryValueDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var repo = new AssetHistoryRepository(context);
            var service = new AssetHistoryService(repo, _mapper);

            // Act
            var result = await service.GetAllWithInclude();

            // Assert
            result.Should().HaveCount(1);
            result[0].Asset.Should().NotBeNull();
            result[0].Asset!.Symbol.Should().Be("GOOGL");
        }
    }
}