using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Features.Assets.Queries.GetAllWithInclude;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.Asset
{
    public class GetAllWithIncludeAssetQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetAllWithIncludeAssetQueryHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"TestDb_GetAllAssets_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AssetMappingProfile>();
                cfg.AddProfile<AssetTypeMappingProfile>();
                cfg.AddProfile<AssetHistoryMappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_Should_Return_Assets_With_AssetType_And_Histories()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var assetType = new Core.Domain.Entities.AssetType { Id = 1, Name = "Crypto", Description = "Crypto asset" };
            context.AssetTypes.Add(assetType);

            var asset = new Core.Domain.Entities.Asset
            {
                Id = 1,
                Name = "Bitcoin",
                Symbol = "BTC",
                AssetTypeId = assetType.Id,
                AssetHistories =
                [
                    new() { Id = 1, Value = 60000m, HistoryValueDate = DateTime.UtcNow,AssetId = 1 }
                ]
            };

            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var repository = new AssetRepository(context);
            var handler = new GetAllWithIncludeAssetQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllWithIncludeAssetQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result[0].Name.Should().Be("Bitcoin");
            result[0].AssetType.Should().NotBeNull();
            result[0].AssetType!.Name.Should().Be("Crypto");
            result[0].AssetHistories.Should().HaveCount(1);
            result[0].AssetHistories!.First().Value.Should().Be(60000m);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Assets_Exist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);
            var handler = new GetAllWithIncludeAssetQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllWithIncludeAssetQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
