using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Queries.GetAllWithIncludeByUser;
using InvestmentApp.Core.Application.Mappings.EntitiesAndDtos;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentPortfolios
{
    public class GetAllInvestmentPortFolioWithIncludeByUserQueryHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;
        private readonly IMapper _mapper;

        public GetAllInvestmentPortFolioWithIncludeByUserQueryHandlerTests()
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
        public async Task Handle_Should_Return_Portfolios_For_Specified_User()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            context.InvestmentPortfolios.AddRange(
                new InvestmentPortfolio { Id = 1, Name = "User1 Portfolio", UserId = "user1" },
                new InvestmentPortfolio { Id = 2, Name = "User1 Portfolio 2", UserId = "user1" },
                new InvestmentPortfolio { Id = 3, Name = "User2 Portfolio", UserId = "user2" }
            );
            await context.SaveChangesAsync();

            var repository = new InvestmentPortfolioRepository(context);
            var handler = new GetAllInvestmentPortFolioWithIncludeByUserQueryHandler(repository, _mapper);

            var query = new GetAllInvestmentPortFolioWithIncludeByUserQuery { UserId = "user1" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(dto => dto.UserId.Should().Be("user1"));
        }

        [Fact]
        public async Task Handle_Should_Return_EmptyList_When_User_Has_No_Portfolios()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            context.InvestmentPortfolios.AddRange(
                new InvestmentPortfolio { Id = 1, Name = "Another Portfolio", UserId = "userX" }
            );
            await context.SaveChangesAsync();

            var repository = new InvestmentPortfolioRepository(context);
            var handler = new GetAllInvestmentPortFolioWithIncludeByUserQueryHandler(repository, _mapper);

            var query = new GetAllInvestmentPortFolioWithIncludeByUserQuery { UserId = "userNotFound" };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
