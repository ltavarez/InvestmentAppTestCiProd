using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Dtos.AssetType;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Application.Services;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Services
{
    public class AssetTypeServiceTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public AssetTypeServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_AssetTypeService_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AssetMappingProfile>();
                cfg.AddProfile<AssetTypeMappingProfile>();
                cfg.AddProfile<AssetHistoryMappingProfile>();
            });
            _mapper = config.CreateMapper();
        }
        private AssetTypeService CreateService()
        {
            var context = new InvestmentAppContext(_dbOptions);
            var repo = new AssetTypeRepository(context);
            return new AssetTypeService(repo, _mapper);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Added_Dto()
        {
            // Arrange
            var service = CreateService();
            var dto = new AssetTypeDto { Id = 0, Name = "Crypto", Description = "Digital" };

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Null_On_Exception()
        {
            // Arrange
            var service = CreateService();
            AssetTypeDto dto = null!;

            // Act 
            var result = await service.AddAsync(dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Entity_When_Exists()
        {
            // Arrange
            var service = CreateService();
            var added = await service.AddAsync(new AssetTypeDto { Id = 0, Name = "Equity" });
            added!.Name = "Updated Equity";

            // Act
            var updated = await service.UpdateAsync(added, added.Id);

            // Assert    
            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Updated Equity");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_Not_Exists()
        {
            // Arrange
            var service = CreateService();
            var dto = new AssetTypeDto { Id = 999, Name = "Ghost" };

            // Act
            var result = await service.UpdateAsync(dto, 999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_When_Deleted()
        {
            //Arrange
            var service = CreateService();
            var dto = await service.AddAsync(new AssetTypeDto { Id = 0, Name = "Temporary" });

            //Act
            var result = await service.DeleteAsync(dto!.Id);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_Exception()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.DeleteAsync(999);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetById_Should_Return_Dto_When_Exists()
        {
            // Arrange
            var service = CreateService();
            var dto = await service.AddAsync(new AssetTypeDto { Id = 0, Name = "Bond" });

            // Act
            var found = await service.GetById(dto!.Id);

            // Assert
            found.Should().NotBeNull();
            found!.Name.Should().Be("Bond");
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
        public async Task GetAll_Should_Return_List_Of_Dtos()
        {
            // Arrange
            var service = CreateService();
            await service.AddAsync(new AssetTypeDto { Id = 0, Name = "A" });
            await service.AddAsync(new AssetTypeDto { Id = 0, Name = "B" });

            // Act
            var result = await service.GetAll();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAll_Should_Return_Empty_When_Exception()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            await context.DisposeAsync();
            var repo = new AssetTypeRepository(context);
            var service = new AssetTypeService(repo, _mapper);

            // Act
            var result = await service.GetAll();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllWithInclude_Should_Return_Data_With_Assets()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var assetType = new AssetType
            {
                Id = 0,
                Name = "Crypto"
            };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var assets = new List<Asset>
            {
                new() {Id = 0, Name = "BTC", Symbol = "BTC", AssetTypeId = assetType.Id },
                new() {Id = 0,  Name = "ETH", Symbol = "ETH", AssetTypeId = assetType.Id }
            };

            context.Assets.AddRange(assets);
            await context.SaveChangesAsync();

            var service = new AssetTypeService(new AssetTypeRepository(context), _mapper);


            //Act
            var result = await service.GetAllWithInclude();

            //Assert
            result.Should().HaveCount(1);
            result[0].AssetQuantity.Should().Be(2);
        }

        [Fact]
        public async Task GetAllWithInclude_Should_Return_Empty_On_Exception()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            await context.DisposeAsync();
            var service = new AssetTypeService(new AssetTypeRepository(context), _mapper);

            // Act
            var result = await service.GetAllWithInclude();

            // Assert
            result.Should().BeEmpty();
        }
    }
}
