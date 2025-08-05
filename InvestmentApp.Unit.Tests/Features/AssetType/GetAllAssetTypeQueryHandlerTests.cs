using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Application.Features.AssetType.Queries.GetAll;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.AssetType
{
    public class GetAllAssetTypeQueryHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public GetAllAssetTypeQueryHandlerTests()
        {       
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AssetMappingProfile>();
                cfg.AddProfile<AssetTypeMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();
       
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldReturnList_WhenAssetTypesExist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            context.AssetTypes.AddRange(
                new Core.Domain.Entities.AssetType { Id = 1, Name = "Crypto", Description = "Criptomonedas" },
                new Core.Domain.Entities.AssetType { Id = 2, Name = "Acciones", Description = "Bolsa" }
            );
            await context.SaveChangesAsync();

            var repository = new AssetTypeRepository(context);
            var handler = new GetAllAssetTypeQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllAssetTypeQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.Name).Should().Contain(["Crypto", "Acciones"]);
        }
    }
}