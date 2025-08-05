using FluentAssertions;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Integration.Tests.Persistence.Repositories
{
    public class InvestmentPortfolioRepositoryTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public InvestmentPortfolioRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_InvestmentPortfolio_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_Add_Portfolio_To_Database()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new InvestmentPortfolioRepository(context);
            var portfolio = new InvestmentPortfolio
            {
                Id = 0,
                UserId = Guid.NewGuid().ToString(),
                Name = "Test Portfolio",
            };

            //Act
            var result = await repo.AddAsync(portfolio);

            //Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AddAsync_Should_Throw_When_Null()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new InvestmentPortfolioRepository(context);

            //Act
            Func<Task> act = async () => await repo.AddAsync(null!);

            //Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetById_Should_Return_Portfolio_When_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var portfolio = new InvestmentPortfolio
            {
                Id = 0,
                UserId = "user123",
                Name = "Test Portfolio"
            };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();
            var repo = new InvestmentPortfolioRepository(context);

            // Act
            var result = await repo.GetById(portfolio.Id);

            // Assert
            result.Should().NotBeNull();
            result!.UserId.Should().Be("user123");
            result!.Name.Should().Be("Test Portfolio");
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new InvestmentPortfolioRepository(context);

            //Act
            var result = await repo.GetById(999);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Existing_Portfolio()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var portfolio = new InvestmentPortfolio
            {
                Id = 0,
                UserId = Guid.NewGuid().ToString(),
                Name = "Initial Portfolio"
            };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var repo = new InvestmentPortfolioRepository(context);
            portfolio.UserId = "updatedUser";

            //Act
            var updated = await repo.UpdateAsync(portfolio.Id, portfolio);

            //Assert
            updated.Should().NotBeNull();
            updated!.UserId.Should().Be("updatedUser");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_NotExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new InvestmentPortfolioRepository(context);

            var fake = new InvestmentPortfolio
            {
                Id = 999,
                UserId = Guid.NewGuid().ToString(),
                Name = "Fake Portfolio"
            };

            // Act
            var result = await repo.UpdateAsync(fake.Id, fake);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Portfolio()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var portfolio = new InvestmentPortfolio
            {
                Id = 0,
                UserId = Guid.NewGuid().ToString(),
                Name = "Portfolio to Delete"
            };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();
            var repo = new InvestmentPortfolioRepository(context);

            // Act
            await repo.DeleteAsync(portfolio.Id);
            var result = await repo.GetById(portfolio.Id);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_NotExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new InvestmentPortfolioRepository(context);

            // Act
            Func<Task> act = async () => await repo.DeleteAsync(999);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllList_Should_Return_All_Portfolios()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.InvestmentPortfolios.AddRange(
                new InvestmentPortfolio { UserId = Guid.NewGuid().ToString(), Id = 0, Name = "Test" },
                new InvestmentPortfolio { UserId = Guid.NewGuid().ToString(), Id = 0, Name = "Test 2" }
            );
            await context.SaveChangesAsync();

            var repo = new InvestmentPortfolioRepository(context);

            //Act
            var result = await repo.GetAllList();

            //Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllListWithInclude_Should_Include_InvestmentAssets()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var assetType = new AssetType { Id = 0, Name = "Crypto", Description = "" };
            var asset = new Asset { Id = 0, AssetTypeId = 1, Name = "BTC", Symbol = "BTC", AssetType = assetType };
            var portfolio = new InvestmentPortfolio
            {
                Id = 0,
                Name = "Portfolio with Assets",
                UserId = Guid.NewGuid().ToString(),
                InvestmentAssets = new List<InvestmentAssets>
                {
                    new InvestmentAssets
                    {
                        AssetId = asset.Id,
                        Id = 0,
                        InvestmentPortfolioId = 0,
                        Asset = asset,
                        AssociationDate = DateTime.UtcNow
                    }
                }
            };

            context.AssetTypes.Add(assetType);
            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();
            var repo = new InvestmentPortfolioRepository(context);

            //Act
            var result = await repo.GetAllListWithInclude(["InvestmentAssets"]);

            //Assert
            result.Should().NotBeEmpty();
            result[0].InvestmentAssets.Should().NotBeNull();
            result[0].InvestmentAssets.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllQuery_Should_Return_Queryable_Portfolios()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            context.InvestmentPortfolios.Add(new InvestmentPortfolio { Id = 0, Name = "Test", UserId = Guid.NewGuid().ToString() });
            await context.SaveChangesAsync();
            var repo = new InvestmentPortfolioRepository(context);

            // Act
            var query = repo.GetAllQuery();
            var result = await query.ToListAsync();

            // Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllQueryWithInclude_Should_Include_InvestmentAssets()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset {Id=0, Name = "ETH", Symbol = "ETH", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio
            {
                Id = 0,
                Name = "Portfolio with Assets",
                UserId =Guid.NewGuid().ToString(),
                InvestmentAssets = new List<InvestmentAssets>
                {
                    new InvestmentAssets
                    {
                        AssetId = asset.Id,
                        Id = 0,
                        InvestmentPortfolioId = 0,
                        Asset = asset,
                        AssociationDate = DateTime.UtcNow
                    }
                }
            };

            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();
            var repo = new InvestmentPortfolioRepository(context);

            // Act
            var query = repo.GetAllQueryWithInclude(["InvestmentAssets"]);
            var result = await query.ToListAsync();

            // Assert
            result.Should().NotBeEmpty();
            result[0].InvestmentAssets.Should().NotBeNull();
            result[0].InvestmentAssets.Should().NotBeEmpty();
        }
    }
}
