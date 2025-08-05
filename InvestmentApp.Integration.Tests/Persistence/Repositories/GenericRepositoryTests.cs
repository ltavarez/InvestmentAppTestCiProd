using FluentAssertions;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Integration.Tests.Persistence.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public GenericRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_GenericRepo_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_Add_Entity()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new GenericRepository<AssetType>(context);
            var entity = new AssetType { Id = 0, Name = "Bond", Description = "Fixed Income" };

            // Act
            var result = await repo.AddAsync(entity);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AddRangeAsync_Should_Add_Multiple_Entities()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new GenericRepository<AssetType>(context);
            var items = new List<AssetType>
            {
                new AssetType {Id = 0, Name = "Crypto" },
                new AssetType {Id = 0, Name = "Stock" }
            };

            // Act
            var result = await repo.AddRangeAsync(items);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetById_Should_Return_Entity_When_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var entity = new AssetType { Id = 0, Name = "ETF" };
            context.AssetTypes.Add(entity);
            await context.SaveChangesAsync();
            var repo = new GenericRepository<AssetType>(context);

            // Act
            var result = await repo.GetById(entity.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("ETF");
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new GenericRepository<AssetType>(context);

            // Act
            var result = await repo.GetById(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Existing_Entity()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var entity = new AssetType { Id = 0, Name = "Mutual Fund" };
            context.AssetTypes.Add(entity);
            await context.SaveChangesAsync();

            var repo = new GenericRepository<AssetType>(context);
            entity.Name = "Updated Fund";

            // Act
            var result = await repo.UpdateAsync(entity.Id, entity);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Updated Fund");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_Entity_Not_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new GenericRepository<AssetType>(context);
            var entity = new AssetType { Id = 999, Name = "Nonexistent" };

            // Act
            var result = await repo.UpdateAsync(999, entity);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Entity()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var entity = new AssetType { Id = 0, Name = "REIT" };
            context.AssetTypes.Add(entity);
            await context.SaveChangesAsync();
            var repo = new GenericRepository<AssetType>(context);

            // Act
            await repo.DeleteAsync(entity.Id);
            var result = await repo.GetById(entity.Id);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Entity_Not_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new GenericRepository<AssetType>(context);

            // Act
            Func<Task> act = async () => await repo.DeleteAsync(999);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllList_Should_Return_All_Entities()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.AssetTypes.AddRange(
                new AssetType { Id = 0, Name = "Cash" },
                new AssetType { Id = 0, Name = "Gold" }
            );
            await context.SaveChangesAsync();
            var repo = new GenericRepository<AssetType>(context);

            // Act
            var result = await repo.GetAllList();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllListWithInclude_Should_Return_Entities_With_Navigation()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.AssetTypes.Add(new AssetType
            {
                Id = 0,
                Name = "Crypto",
                Assets =
                [
                    new Asset {Id = 0,  Name = "BTC", Symbol = "BTC",AssetTypeId = 1 }
                ]
            });
            await context.SaveChangesAsync();
            var repo = new GenericRepository<AssetType>(context);

            // Act
            var result = await repo.GetAllListWithInclude(["Assets"]);

            // Assert
            result.Should().NotBeEmpty();
            result[0].Assets.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllQuery_Should_Return_Queryable_Entities()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.AssetTypes.Add(new AssetType { Id = 0, Name = "Index Fund" });
            await context.SaveChangesAsync();

            var repo = new GenericRepository<AssetType>(context);

            // Act
            var query = repo.GetAllQuery();
            var result = await query.ToListAsync();

            // Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllQueryWithInclude_Should_Return_Queryable_With_Navigation()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.AssetTypes.Add(new AssetType
            {
                Id = 0,
                Name = "Currency",
                Assets =
                [
                    new Asset {Id = 0,  Name = "USD", Symbol = "USD",AssetTypeId = 0 }
                ]
            });
            await context.SaveChangesAsync();
            var repo = new GenericRepository<AssetType>(context);

            // Act
            var query = repo.GetAllQueryWithInclude(["Assets"]);
            var result = await query.ToListAsync();

            // Assert
            result.Should().NotBeEmpty();
            result[0].Assets.Should().NotBeEmpty();
        }
    }
}
