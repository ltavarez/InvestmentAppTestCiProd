using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.InvestmentAssets.Queries.GetById;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentAssets
{
    public class GetByIdInvestmentAssetQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetByIdInvestmentAssetQueryHandlerTests()
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
        public async Task Handle_Should_Return_Dto_When_Valid_Id_And_UserId()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var userId = "user-test";

            var assetType = new Core.Domain.Entities.AssetType { Id = 0, Name = "Equity" };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var portfolio = new InvestmentPortfolio { Id = 1, UserId = userId, Name = "Portfolio 1" };
            var asset = new Core.Domain.Entities.Asset { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = assetType.Id };
            var investmentAsset = new Core.Domain.Entities.InvestmentAssets
            {
                Id = 5,
                AssetId = asset.Id,
                InvestmentPortfolioId = portfolio.Id,
                Asset = asset,
                InvestmentPortfolio = portfolio,
                AssociationDate = DateTime.UtcNow
            };

            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            context.InvestmentAssets.Add(investmentAsset);
            await context.SaveChangesAsync();

            var handler = new GetByIdInvestmentAssetQueryHandler(new InvestmentAssetRepository(context), _mapper);

            var query = new GetByIdInvestmentAssetQuery
            {
                Id = investmentAsset.Id,
                UserId = userId
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(investmentAsset.Id);
            result.AssetId.Should().Be(asset.Id);
            result.InvestmentPortfolioId.Should().Be(portfolio.Id);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Invalid_Id_Or_UserId()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var handler = new GetByIdInvestmentAssetQueryHandler(new InvestmentAssetRepository(context), _mapper);

            var query = new GetByIdInvestmentAssetQuery
            {
                Id = 999,
                UserId = "not-found"
            };

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Investment Assets not found with this id");
        }
    }
}
