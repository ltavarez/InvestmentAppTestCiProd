using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.AssetsHistories.Queries.GetById;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.AssetsHistories
{
    public class GetByIdAssetHistoryQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetByIdAssetHistoryQueryHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"Db_GetByIdAssetHistory_{Guid.NewGuid()}")
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
        public async Task Handle_ShouldReturnDto_WhenAssetHistoryExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var asset = new Core.Domain.Entities.Asset { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = 1 };
            context.Assets.Add(asset);

            var assetHistory = new AssetHistory
            {
                Id = 1,
                AssetId = asset.Id,
                Value = 45250.75m,
                HistoryValueDate = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc),
            };

            context.AssetHistories.Add(assetHistory);
            await context.SaveChangesAsync();

            var repository = new AssetHistoryRepository(context);
            var handler = new GetByIdAssetHistoryQueryHandler(repository, _mapper);

            var query = new GetByIdAssetHistoryQuery { Id = assetHistory.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(assetHistory.Id);
            result.Value.Should().Be(assetHistory.Value);
            result.Asset.Should().NotBeNull();
            result.Asset!.Name.Should().Be("Bitcoin");
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenAssetHistoryDoesNotExist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetHistoryRepository(context);
            var handler = new GetByIdAssetHistoryQueryHandler(repository, _mapper);

            var query = new GetByIdAssetHistoryQuery { Id = 999 };

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Asset history not found with this id");
        }
    }
}
