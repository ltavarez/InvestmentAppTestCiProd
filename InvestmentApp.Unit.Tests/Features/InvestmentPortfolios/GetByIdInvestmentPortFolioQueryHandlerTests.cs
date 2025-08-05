using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Queries.GetById;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentPortfolios
{
    public class GetByIdInvestmentPortFolioQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetByIdInvestmentPortFolioQueryHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"Db_{Guid.NewGuid()}")
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

        [Fact]
        public async Task Handle_Should_Return_Portfolio_When_Found()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var portfolio = new InvestmentPortfolio
            {
                Id = 1,
                Name = "Growth Portfolio",
                Description = "Long-term investments",
                UserId = "user123"
            };
            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var repository = new InvestmentPortfolioRepository(context);
            var handler = new GetByIdInvestmentPortFolioQueryHandler(repository, _mapper);

            var query = new GetByIdInvestmentPortFolioQuery
            {
                Id = 1,
                UserId = "user123"
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Growth Portfolio");
            result.UserId.Should().Be("user123");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Portfolio_Not_Found()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new InvestmentPortfolioRepository(context);
            var handler = new GetByIdInvestmentPortFolioQueryHandler(repository, _mapper);

            var query = new GetByIdInvestmentPortFolioQuery
            {
                Id = 99,
                UserId = "userXYZ"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ApiException>()
                .WithMessage("Investment Portfolio not found with this id");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_UserId_Does_Not_Match()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            context.InvestmentPortfolios.Add(new InvestmentPortfolio
            {
                Id = 10,
                Name = "Mismatch Portfolio",
                UserId = "user1"
            });
            await context.SaveChangesAsync();

            var repository = new InvestmentPortfolioRepository(context);
            var handler = new GetByIdInvestmentPortFolioQueryHandler(repository, _mapper);

            var query = new GetByIdInvestmentPortFolioQuery
            {
                Id = 10,
                UserId = "anotherUser"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<ApiException>()
                .WithMessage("Investment Portfolio not found with this id");
        }
    }
}
