using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Features.Assets.Queries.GetAll;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.Asset
{
    public class GetAllAssetQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetAllAssetQueryHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"Db_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AssetMappingProfile>();
                cfg.AddProfile<AssetTypeMappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_Should_Return_All_Assets_With_AssetType()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var type1 = new Core.Domain.Entities.AssetType { Id = 1, Name = "Crypto", Description = "Digital assets" };
            var type2 = new Core.Domain.Entities.AssetType { Id = 2, Name = "Fiat", Description = "Gov currencies" };

            context.AssetTypes.AddRange(type1, type2);

            context.Assets.AddRange(
                new Core.Domain.Entities.Asset { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = 1 },
                new Core.Domain.Entities.Asset { Id = 2, Name = "Ethereum", Symbol = "ETH", AssetTypeId = 1 },
                new Core.Domain.Entities.Asset { Id = 3, Name = "Dollar", Symbol = "USD", AssetTypeId = 2 }
            );

            await context.SaveChangesAsync();

            var repository = new AssetRepository(context);
            var handler = new GetAllAssetQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllAssetQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
            result.All(a => a.AssetType is not null).Should().BeTrue();
            result.First(a => a.Symbol == "BTC").AssetType!.Name.Should().Be("Crypto");
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Assets_Exist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);
            var handler = new GetAllAssetQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllAssetQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
