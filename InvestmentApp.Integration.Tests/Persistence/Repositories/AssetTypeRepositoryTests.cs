using FluentAssertions;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Integration.Tests.Persistence.Repositories
{
    public class AssetTypeRepositoryTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public AssetTypeRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_AssetTypes_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_Add_AssetType_To_Database()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);
            var assetType = new AssetType { Name = "Crypto", Description = "Cryptocurrency", Id = 0 };

            // Act
            var result = await repository.AddAsync(assetType);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
            var assetTypes = await context.AssetTypes.ToListAsync();
            assetTypes.Should().ContainSingle();
        }

        [Fact]
        public async Task AddAsync_Should_Throw_When_Null()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);

            // Act
            Func<Task> act = async () => await repository.AddAsync(null!);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetById_Should_Return_AssetType_When_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var assetType = new AssetType { Name = "Equity", Description = "Stock investments", Id = 0 };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();
            var repository = new AssetTypeRepository(context);

            // Act
            var result = await repository.GetById(assetType.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Equity");
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);

            // Act
            var result = await repository.GetById(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_AssetType()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var assetType = new AssetType { Name = "Bond", Description = "Fixed income", Id = 0 };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();

            var repository = new AssetTypeRepository(context);
            assetType.Name = "Updated Bond";

            // Act
            var updated = await repository.UpdateAsync(assetType.Id, assetType);

            // Assert
            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Updated Bond");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_AssetType_Not_Found()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);
            var fake = new AssetType { Id = 999, Name = "Nonexistent", Description = "X" };

            // Act
            var result = await repository.UpdateAsync(fake.Id, fake);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_AssetType()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var assetType = new AssetType { Name = "Real Estate", Description = "Property", Id = 0 };
            context.AssetTypes.Add(assetType);
            await context.SaveChangesAsync();
            var repository = new AssetTypeRepository(context);

            // Act
            await repository.DeleteAsync(assetType.Id);
            var entity = await repository.GetById(assetType.Id);

            // Assert
            entity.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_Not_Found()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);

            // Act
            Func<Task> act = async () => await repository.DeleteAsync(999);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllList_Should_Return_All_AssetTypes()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.AssetTypes.AddRange(
                new AssetType { Name = "Crypto", Id = 0 },
                new AssetType { Name = "Stocks", Id = 0 });
            await context.SaveChangesAsync();
            var repository = new AssetTypeRepository(context);

            //Act
            var result = await repository.GetAllList();

            //Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllList_Should_Return_Empty_When_No_AssetTypes()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);

            // Act
            var result = await repository.GetAllList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllListWithInclude_ShouldIncludeAssets()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.AssetTypes.Add(new AssetType
            {
                Id = 1,
                Name = "Crypto",
                Description = "",
                Assets = [new Asset { Id = 1, Name = "BTC", Symbol = "BTC", AssetTypeId = 1 }]
            });
            await context.SaveChangesAsync();
            var repo = new AssetTypeRepository(context);

            // Act
            var result = await repo.GetAllListWithInclude(["Assets"]);

            // Assert
            result.Should().NotBeEmpty();
            result[0].Assets.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllQuery_ShouldReturnQueryableAssetTypes()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.AssetTypes.Add(new AssetType { Id = 1, Name = "Crypto", Description = "" });
            await context.SaveChangesAsync();
            var repo = new AssetTypeRepository(context);

            // Act
            var query = repo.GetAllQuery();
            var result = await query.ToListAsync();

            // Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllQueryWithInclude_ShouldIncludeAssets()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.AssetTypes.Add(new AssetType
            {
                Id = 1,
                Name = "Crypto",
                Description = "",
                Assets = [new Asset { Id = 1, Name = "BTC", Symbol = "BTC", AssetTypeId = 1 }]
            });
            await context.SaveChangesAsync();
            var repo = new AssetTypeRepository(context);

            // Act
            var query = repo.GetAllQueryWithInclude(["Assets"]);
            var result = await query.ToListAsync();

            // Assert
            result.Should().NotBeEmpty();
            result[0].Assets.Should().NotBeEmpty();
        }
    }
}
