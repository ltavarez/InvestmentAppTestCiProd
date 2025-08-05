using FluentAssertions;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Integration.Tests.Persistence.Repositories
{
    public class AssetRepositoryTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public AssetRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Assets_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_Add_Asset_To_Database()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);
            var asset = new Asset { Name = "Bitcoin", Symbol = "BTC", AssetTypeId = 1, Id = 0 };

            // Act
            var result = await repository.AddAsync(asset);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
            var assets = await context.Assets.ToListAsync();
            assets.Should().ContainSingle();
        }

        [Fact]
        public async Task AddAsync_Should_Not_Add_Null_Asset()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);

            //Act
            Func<Task> act = async () => await repository.AddAsync(null!);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetById_Should_Return_Asset_When_Exists()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Name = "Ethereum", Symbol = "ETH", AssetTypeId = 1, Id = 0 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();
            var repository = new AssetRepository(context);

            //Act
            var result = await repository.GetById(asset.Id);


            //Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Ethereum");
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);

            // Act
            var result = await repository.GetById(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_Asset()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Name = "Ripple", Symbol = "XRP", AssetTypeId = 1, Id = 0 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var repository = new AssetRepository(context);
            asset.Name = "Updated Ripple";

            // Act
            var updated = await repository.UpdateAsync(asset.Id, asset);

            // Assert
            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Updated Ripple");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_Asset_Not_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);
            var asset = new Asset { Id = 999, Name = "Fake", Symbol = "X", AssetTypeId = 1 };

            // Act
            var result = await repository.UpdateAsync(asset.Id, asset);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Asset()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Name = "Litecoin", Symbol = "LTC", AssetTypeId = 1, Id = 0 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();
            var repository = new AssetRepository(context);

            // Act
            await repository.DeleteAsync(asset.Id);
            var entity = await repository.GetById(asset.Id);

            // Assert
            entity.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_Not_Found()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);

            // Act
            Func<Task> act = async () => await repository.DeleteAsync(999);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllList_Should_Return_All_Assets()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.Assets.AddRange(
                new Asset { Name = "Bitcoin", Symbol = "BTC", AssetTypeId = 1, Id = 0 },
                new Asset { Name = "Ethereum", Symbol = "ETH", AssetTypeId = 1, Id = 0 });
            await context.SaveChangesAsync();
            var repository = new AssetRepository(context);

            //Act
            var result = await repository.GetAllList();

            //Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllList_Should_Return_Empty_When_No_Assets()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetRepository(context);

            // Act
            var result = await repository.GetAllList();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllListWithInclude_ShouldIncludeAssetType()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.Assets.Add(new Asset
            {
                Id = 1,
                Name = "Bitcoin",
                Symbol = "BTC",
                AssetTypeId = 1,
                AssetType = new AssetType { Id = 1, Name = "Crypto", Description = "" }
            });
            await context.SaveChangesAsync();
            var repo = new AssetRepository(context);

            // Act
            var result = await repo.GetAllListWithInclude(["AssetType"]);

            // Assert
            result.Should().NotBeEmpty();
            result[0].AssetType.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllQuery_ShouldReturnQueryableAssets()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.Assets.Add(new Asset { Id = 1, Name = "Bitcoin", Symbol = "BTC", AssetTypeId = 1 });
            await context.SaveChangesAsync();
            var repo = new AssetRepository(context);

            // Act
            var query = repo.GetAllQuery();
            var result = await query.ToListAsync();

            // Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllQueryWithInclude_ShouldIncludeAssetType()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.Assets.Add(new Asset
            {
                Id = 1,
                Name = "Bitcoin",
                Symbol = "BTC",
                AssetTypeId = 1,
                AssetType = new AssetType { Id = 1, Name = "Crypto", Description = "" }
            });
            await context.SaveChangesAsync();
            var repo = new AssetRepository(context);

            // Act
            var query = repo.GetAllQueryWithInclude(["AssetType"]);
            var result = await query.ToListAsync();

            // Assert
            result.Should().NotBeEmpty();
            result[0].AssetType.Should().NotBeNull();
        }
    }
}
