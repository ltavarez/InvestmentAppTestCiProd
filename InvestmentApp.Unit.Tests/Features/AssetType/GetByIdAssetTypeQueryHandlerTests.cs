using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Dtos.Asset;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.AssetType.Queries.GetById;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.AssetType
{
    public class GetByIdAssetTypeQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetByIdAssetTypeQueryHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"InMemoryDb_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AssetMappingProfile>();
                cfg.AddProfile<AssetTypeMappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldReturnAssetTypeResponseDto_WhenIdExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var assetType = new Core.Domain.Entities.AssetType
            {
                Id = 1,
                Name = "Crypto",
                Description = "Digital Currency",
                Assets = [
                    new() { Id = 1, Name = "Bitcoin", Description = "BTC", Symbol = "BTC", AssetTypeId = 1 }
                ]
            };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var repository = new AssetTypeRepository(context);
            var handler = new GetByIdAssetTypeQueryHandler(repository, _mapper);

            var query = new GetByIdAssetTypeQuery { Id = 1 };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Crypto");
            result.Assets.Should().HaveCount(1);
            result.Assets?[0].Name.Should().Be("Bitcoin");
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenIdDoesNotExist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);
            var handler = new GetByIdAssetTypeQueryHandler(repository, _mapper);

            var query = new GetByIdAssetTypeQuery { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Asset type not found with this id");
        }
    }
}
