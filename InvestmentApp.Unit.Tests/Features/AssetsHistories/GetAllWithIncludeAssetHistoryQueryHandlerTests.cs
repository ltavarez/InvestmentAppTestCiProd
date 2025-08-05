using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Features.AssetsHistories.Queries.GetAllWithInclude;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.AssetsHistories
{
    public class GetAllWithIncludeAssetHistoryQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetAllWithIncludeAssetHistoryQueryHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"Db_GetAllAssetHistory_{Guid.NewGuid()}")
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
        public async Task Handle_ShouldReturnAssetHistoriesWithAssets()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var assetType = new Core.Domain.Entities.AssetType { Id = 0, Name = "Equity" };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var asset = new Core.Domain.Entities.Asset
            {
                Id = 1,
                Name = "Bitcoin",
                Symbol = "BTC",
                AssetTypeId = assetType.Id,
            };

            context.Assets.Add(asset);

            var history1 = new Core.Domain.Entities.AssetHistory
            {
                Id = 1,
                Value = 50000m,
                HistoryValueDate = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                AssetId = asset.Id
            };

            var history2 = new  Core.Domain.Entities.AssetHistory
            {
                Id = 2,
                Value = 48000m,
                HistoryValueDate = new DateTime(2024, 12, 30, 0, 0, 0, DateTimeKind.Utc),
                AssetId = asset.Id
            };

            context.AssetHistories.AddRange(history1, history2);
            await context.SaveChangesAsync();

            var repository = new AssetHistoryRepository(context);
            var handler = new GetAllWithIncludeAssetHistoryQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllWithIncludeAssetHistoryQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Asset.Should().NotBeNull();
            result[0].Asset!.Name.Should().Be("Bitcoin");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoAssetHistoriesExist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetHistoryRepository(context);
            var handler = new GetAllWithIncludeAssetHistoryQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllWithIncludeAssetHistoryQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
