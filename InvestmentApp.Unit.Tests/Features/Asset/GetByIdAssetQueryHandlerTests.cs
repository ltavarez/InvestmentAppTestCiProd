using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.Assets.Queries.GetById;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.Asset
{
    public class GetByIdAssetQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetByIdAssetQueryHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"Db_{Guid.NewGuid()}")
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
        public async Task Handle_Should_Return_AssetDto_When_Id_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var assetType = new Core.Domain.Entities.AssetType { Id = 1, Name = "Crypto", Description = "Digital" };
            var asset = new Core.Domain.Entities.Asset
            {
                Id = 1,
                Name = "Bitcoin",
                Symbol = "BTC",
                AssetTypeId = assetType.Id,
                AssetType = assetType,
                AssetHistories =
                [
                    new() { Id = 1, Value = 60000m, HistoryValueDate = DateTime.UtcNow,AssetId = 1 }
                ]
            };

            context.AssetTypes.Add(assetType);
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var repository = new AssetRepository(context);
            var handler = new GetByIdAssetQueryHandler(repository, _mapper);
            var query = new GetByIdAssetQuery { Id = asset.Id };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(asset.Id);
            result.Name.Should().Be(asset.Name);
            result.Symbol.Should().Be(asset.Symbol);
            result.AssetType.Should().NotBeNull();
            result.AssetType!.Name.Should().Be(assetType.Name);
            result.AssetHistories.Should().HaveCount(1);
            result.AssetHistories.First().Value.Should().Be(60000m);
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Asset_Not_Found()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);
            var handler = new GetByIdAssetQueryHandler(repository, _mapper);
            var query = new GetByIdAssetQuery { Id = 999 };

            // Act & Assert
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>().WithMessage("Asset not found with this id");
        }
    }
}
