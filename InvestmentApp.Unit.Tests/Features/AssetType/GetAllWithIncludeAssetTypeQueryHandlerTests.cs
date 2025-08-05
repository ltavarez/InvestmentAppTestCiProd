using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Features.AssetType.Queries.GetAllWithInclude;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.AssetType
{
    public class GetAllWithIncludeAssetTypeQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetAllWithIncludeAssetTypeQueryHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"Db_{Guid.NewGuid()}")
                .Options;

            // AutoMapper config
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AssetMappingProfile>();
                cfg.AddProfile<AssetTypeMappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldReturnAssetTypesWithAssets()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var assetType = new Core.Domain.Entities.AssetType
            {
                Id = 1,
                Name = "Crypto",
                Description = "Digital currencies",
                Assets =
                [
                    new() { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = 1 },
                    new() { Id = 2, Name = "Ethereum", Symbol = "ETH", AssetTypeId = 1 }
                ]
            };

            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var repository = new AssetTypeRepository(context);
            var handler = new GetAllWithIncludeAssetTypeQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllWithIncludeAssetTypeQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].Assets.Should().HaveCount(2);
            result[0].Name.Should().Be("Crypto");
            result[0].Assets?[0].Symbol.Should().Be("BTC");
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoAssetTypesExist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);
            var handler = new GetAllWithIncludeAssetTypeQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllWithIncludeAssetTypeQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
