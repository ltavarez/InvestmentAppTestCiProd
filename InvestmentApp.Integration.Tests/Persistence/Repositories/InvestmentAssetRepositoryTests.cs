using FluentAssertions;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Integration.Tests.Persistence.Repositories
{
    public class InvestmentAssetRepositoryTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public InvestmentAssetRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_InvestmentAsset_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task AddAsync_Should_Add_InvestmentAsset_To_Database()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "BTC", Symbol = "BTC", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, UserId = Guid.NewGuid().ToString(), Name = "Growth Portfolio" };
            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var repo = new InvestmentAssetRepository(context);
            var entity = new InvestmentAssets
            {
                Id = 0,
                AssetId = asset.Id,
                InvestmentPortfolioId = portfolio.Id,
                AssociationDate = DateTime.UtcNow
            };

            //Act
            var result = await repo.AddAsync(entity);

            //Assert
            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task AddAsync_Should_Throw_When_Null()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new InvestmentAssetRepository(context);

            //Act
            Func<Task> act = async () => await repo.AddAsync(null!);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task GetById_Should_Return_Entity_When_Exists()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "ETH", Symbol = "ETH", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, UserId = Guid.NewGuid().ToString(), Name = "Income Portfolio" };
            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var entity = new InvestmentAssets
            {
                Id = 0,
                AssetId = asset.Id,
                InvestmentPortfolioId = portfolio.Id
            };
            context.InvestmentAssets.Add(entity);
            await context.SaveChangesAsync();
            var repo = new InvestmentAssetRepository(context);

            //Act
            var result = await repo.GetById(entity.Id);

            //Assert
            result.Should().NotBeNull();
            result!.AssetId.Should().Be(asset.Id);
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_NotExists()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new InvestmentAssetRepository(context);

            //Act
            var result = await repo.GetById(999);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Existing_Entity()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "USD", Symbol = "USD", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, UserId = Guid.NewGuid().ToString(), Name = "Stable Portfolio" };
            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var entity = new InvestmentAssets
            {
                Id = 0,
                AssetId = asset.Id,
                InvestmentPortfolioId = portfolio.Id,
                AssociationDate = DateTime.UtcNow.AddDays(-10)
            };
            context.InvestmentAssets.Add(entity);
            await context.SaveChangesAsync();

            var repo = new InvestmentAssetRepository(context);
            entity.AssociationDate = DateTime.UtcNow;

            //Act
            var updated = await repo.UpdateAsync(entity.Id, entity);

            //Assert
            updated.Should().NotBeNull();
            updated!.AssociationDate.Date.Should().Be(DateTime.UtcNow.Date);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_When_NotExists()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new InvestmentAssetRepository(context);

            var fake = new InvestmentAssets
            {
                Id = 999,
                AssetId = 1,
                InvestmentPortfolioId = 1,
                AssociationDate = DateTime.UtcNow
            };

            //Act
            var result = await repo.UpdateAsync(fake.Id, fake);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Entity()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "Gold", Symbol = "XAU", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, UserId = Guid.NewGuid().ToString(), Name = "Wealth Portfolio" };
            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var entity = new InvestmentAssets
            {
                Id = 0,
                AssetId = asset.Id,
                InvestmentPortfolioId = portfolio.Id
            };
            context.InvestmentAssets.Add(entity);
            await context.SaveChangesAsync();
            var repo = new InvestmentAssetRepository(context);

            //Act
            await repo.DeleteAsync(entity.Id);
            var result = await repo.GetById(entity.Id);

            //Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_NotExists()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repo = new InvestmentAssetRepository(context);

            //Act
            Func<Task> act = async () => await repo.DeleteAsync(999);

            //Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAllList_Should_Return_All_Entities()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "Silver", Symbol = "XAG", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, UserId = Guid.NewGuid().ToString(), Name = "Diversified Portfolio" };
            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            context.InvestmentAssets.AddRange(
                new InvestmentAssets { Id = 0, AssetId = asset.Id, InvestmentPortfolioId = portfolio.Id },
                new InvestmentAssets { Id = 0, AssetId = asset.Id, InvestmentPortfolioId = portfolio.Id }
            );
            await context.SaveChangesAsync();
            var repo = new InvestmentAssetRepository(context);

            //Act
            var result = await repo.GetAllList();

            //Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllListWithInclude_Should_IncludeAssetAndPortfolio()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "Oil", Symbol = "OIL", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, UserId = Guid.NewGuid().ToString(), Name = "Energy Portfolio" };
            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            context.InvestmentAssets.Add(new InvestmentAssets
            {
                Id = 0,
                AssetId = asset.Id,
                InvestmentPortfolioId = portfolio.Id,
                Asset = asset,
                InvestmentPortfolio = portfolio
            });
            await context.SaveChangesAsync();
            var repo = new InvestmentAssetRepository(context);

            //Act
            var result = await repo.GetAllListWithInclude(["Asset", "InvestmentPortfolio"]);

            //Assert
            result.Should().NotBeEmpty();
            result[0].Asset.Should().NotBeNull();
            result[0].InvestmentPortfolio.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllQuery_Should_Return_Queryable_Entities()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "EUR", Symbol = "EUR", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, UserId = Guid.NewGuid().ToString(), Name = "Forex Portfolio" };
            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            context.InvestmentAssets.Add(new InvestmentAssets
            {
                Id = 0,
                AssetId = asset.Id,
                InvestmentPortfolioId = portfolio.Id
            });
            await context.SaveChangesAsync();

            var repo = new InvestmentAssetRepository(context);

            //Act
            var query = repo.GetAllQuery();
            var result = await query.ToListAsync();

            //Assert
            result.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetAllQueryWithInclude_Should_IncludeAssetAndPortfolio()
        {
            //Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var asset = new Asset { Id = 0, Name = "JPY", Symbol = "JPY", AssetTypeId = 1 };
            var portfolio = new InvestmentPortfolio { Id = 0, UserId = Guid.NewGuid().ToString(), Name = "Currency Portfolio" };
            context.Assets.Add(asset);
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            context.InvestmentAssets.Add(new InvestmentAssets
            {
                Id = 0,
                AssetId = asset.Id,
                InvestmentPortfolioId = portfolio.Id,
                Asset = asset,
                InvestmentPortfolio = portfolio
            });
            await context.SaveChangesAsync();

            var repo = new InvestmentAssetRepository(context);

            //Act
            var query = repo.GetAllQueryWithInclude(["Asset", "InvestmentPortfolio"]);
            var result = await query.ToListAsync();

            //Assert
            result.Should().NotBeEmpty();
            result[0].Asset.Should().NotBeNull();
            result[0].InvestmentPortfolio.Should().NotBeNull();
        }
    }
}
