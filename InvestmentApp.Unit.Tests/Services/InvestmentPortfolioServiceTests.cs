using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Application.Services;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Services
{
    public class InvestmentPortfolioServiceTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public InvestmentPortfolioServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"TestDb_InvestmentPortfolioService_{Guid.NewGuid()}")
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

        private InvestmentPortfolioService CreateService()
        {
            var context = new InvestmentAppContext(_dbOptions);
            var repository = new InvestmentPortfolioRepository(context);
            return new InvestmentPortfolioService(repository, _mapper);
        }

        [Fact]
        public async Task GetById_Should_Return_Portfolio_When_Exists()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var portfolio = new InvestmentPortfolio { Id = 0, Name = "Test Portfolio", UserId = "user1" };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var service = CreateService();

            // Act
            var result = await service.GetById(portfolio.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(portfolio.Id);
        }

        [Fact]
        public async Task GetById_Should_Return_Null_When_Not_Exists()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.GetById(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetById_With_UserId_Should_Return_If_Match()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var portfolio = new InvestmentPortfolio { Id = 0, Name = "User Portfolio", UserId = "user42" };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var service = CreateService();

            // Act
            var result = await service.GetById(portfolio.Id, "user42");

            // Assert
            result.Should().NotBeNull();
            result!.UserId.Should().Be("user42");
        }

        [Fact]
        public async Task GetById_With_UserId_Should_Return_Null_When_UserId_Mismatch()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var portfolio = new InvestmentPortfolio { Id = 0, Name = "No Match", UserId = "realUser" };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var service = CreateService();

            // Act
            var result = await service.GetById(portfolio.Id, "fakeUser");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllWithIncludeByUser_Should_Return_Portfolios_For_User()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            var userId = "user123";

            context.InvestmentPortfolios.AddRange(
                new InvestmentPortfolio { Id = 0, Name = "Portfolio A", UserId = userId },
                new InvestmentPortfolio { Id = 0, Name = "Portfolio B", UserId = userId },
                new InvestmentPortfolio { Id = 0, Name = "Other Portfolio", UserId = "anotherUser" }
            );
            await context.SaveChangesAsync();

            var service = CreateService();

            // Act
            var result = await service.GetAllWithIncludeByUser(userId);

            // Assert
            result.Should().HaveCount(2);
            result.All(p => p.UserId == userId).Should().BeTrue();
        }

        [Fact]
        public async Task GetAllWithIncludeByUser_Should_Return_Empty_When_None_Match()
        {
            // Arrange
            var context = new InvestmentAppContext(_dbOptions);
            context.InvestmentPortfolios.Add(new InvestmentPortfolio { Id = 0, Name = "Test", UserId = "anotherUser" });
            await context.SaveChangesAsync();

            var service = CreateService();

            // Act
            var result = await service.GetAllWithIncludeByUser("unknown_user");

            // Assert
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetAllWithIncludeByUser_Should_Return_Empty_When_UserId_Is_Null_Or_Empty(string? userId)
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.GetAllWithIncludeByUser(userId!);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
