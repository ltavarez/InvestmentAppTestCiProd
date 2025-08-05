using FluentAssertions;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Integration.Tests.Persistence.Repositories
{
    public class AssetHistoryRepositoryTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public AssetHistoryRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_AssetHistory_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_Add_AssetHistory_To_Database()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new AssetHistoryRepository(context);
            var asset = new Asset { Name = "BTC", Symbol = "BTC", AssetTypeId = 1, Id = 0 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var assetHistory = new AssetHistory
            {
                Id = 0,
                HistoryValueDate = DateTime.UtcNow,
                Value = 45000,
                AssetId = asset.Id
            };

            // Act
            var result = await repo.AddAsync(assetHistory);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AddAsync_Should_Throw_When_Null()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new AssetHistoryRepository(context);

            // Act
            Func<Task> act = async () => await repo.AddAsync(null!);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetById_Should_Return_AssetHistory_When_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Name = "ETH", Symbol = "ETH", AssetTypeId = 1, Id = 0 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var history = new AssetHistory
            {
                Id = 0,
                HistoryValueDate = DateTime.UtcNow,
                Value = 2000,
                AssetId = asset.Id
            };
            context.AssetHistories.Add(history);
            await context.SaveChangesAsync();
            var repo = new AssetHistoryRepository(context);

            // Act
            var result = await repo.GetById(history.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Value.Should().Be(2000);
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new AssetHistoryRepository(context);

            // Act
            var result = await repo.GetById(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_AssetHistory()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "USD", Symbol = "USD", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var history = new AssetHistory
            {
                Id = 0,
                HistoryValueDate = DateTime.UtcNow.AddDays(-1),
                Value = 1.0m,
                AssetId = asset.Id
            };
            context.AssetHistories.Add(history);
            await context.SaveChangesAsync();

            var repo = new AssetHistoryRepository(context);
            history.Value = 1.1m;

            // Act
            var updated = await repo.UpdateAsync(history.Id, history);

            // Assert
            updated.Should().NotBeNull();
            updated!.Value.Should().Be(1.1m);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_NotExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new AssetHistoryRepository(context);

            var fake = new AssetHistory
            {
                Id = 999,
                HistoryValueDate = DateTime.UtcNow,
                Value = 999,
                AssetId = 1
            };

            // Act
            var result = await repo.UpdateAsync(fake.Id, fake);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_AssetHistory()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "Gold", Symbol = "XAU", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            var history = new AssetHistory
            {
                Id = 0,
                HistoryValueDate = DateTime.UtcNow,
                Value = 1800,
                AssetId = asset.Id
            };
            context.AssetHistories.Add(history);
            await context.SaveChangesAsync();
            var repo = new AssetHistoryRepository(context);

            // Act
            await repo.DeleteAsync(history.Id);
            var result = await repo.GetById(history.Id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_NotExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new AssetHistoryRepository(context);

            // Act
            Func<Task> act = async () => await repo.DeleteAsync(999);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllList_Should_Return_All_AssetHistories()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "Silver", Symbol = "XAG", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            context.AssetHistories.AddRange(
                new AssetHistory { Id = 0, HistoryValueDate = DateTime.UtcNow, Value = 25, AssetId = asset.Id },
                new AssetHistory { Id = 0, HistoryValueDate = DateTime.UtcNow.AddDays(-1), Value = 24, AssetId = asset.Id }
            );
            await context.SaveChangesAsync();
            var repo = new AssetHistoryRepository(context);

            // Act
            var result = await repo.GetAllList();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllListWithInclude_ShouldIncludeAsset()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "Oil", Symbol = "OIL", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            context.AssetHistories.Add(new AssetHistory
            {
                Id = 0,
                HistoryValueDate = DateTime.UtcNow,
                Value = 70,
                AssetId = asset.Id,
                Asset = asset
            });
            await context.SaveChangesAsync();
            var repo = new AssetHistoryRepository(context);

            //Act
            var result = await repo.GetAllListWithInclude(["Asset"]);

            //Assert
            result.Should().NotBeEmpty();
            result[0].Asset.Should().NotBeNull();
            result[0].Asset!.Name.Should().Be("Oil");
        }

        [Fact]
        public async Task GetAllQuery_ShouldReturnQueryableAssetHistories()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "EUR", Symbol = "EUR", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            context.AssetHistories.Add(new AssetHistory
            {
                Id = 0,
                HistoryValueDate = DateTime.UtcNow,
                Value = 1.2m,
                AssetId = asset.Id
            });
            await context.SaveChangesAsync();
            var repo = new AssetHistoryRepository(context);

            // Act
            var query = repo.GetAllQuery();
            var result = await query.ToListAsync();

            // Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllQueryWithInclude_ShouldIncludeAsset()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "JPY", Symbol = "JPY", AssetTypeId = 1 };
            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            context.AssetHistories.Add(new AssetHistory
            {
                Id = 0,
                HistoryValueDate = DateTime.UtcNow,
                Value = 110,
                AssetId = asset.Id,
                Asset = asset
            });
            await context.SaveChangesAsync();
            var repo = new AssetHistoryRepository(context);

            // Act
            var query = repo.GetAllQueryWithInclude(["Asset"]);
            var result = await query.ToListAsync();

            // Assert
            result.Should().NotBeEmpty();
            result[0].Asset.Should().NotBeNull();
        }
    }
}
