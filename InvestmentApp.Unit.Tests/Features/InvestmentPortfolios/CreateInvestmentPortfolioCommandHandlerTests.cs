using FluentAssertions;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.CreateInvestmentPortfolio;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentPortfolios
{
    public class CreateInvestmentPortfolioCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public CreateInvestmentPortfolioCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"Db_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_Return_Id_When_Portfolio_Is_Created()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var repository = new InvestmentPortfolioRepository(context);
            var handler = new CreateInvestmentPortfolioCommandHandler(repository, null!);

            var command = new CreateInvestmentPortfolioCommand
            {
                Name = "Cartera Cripto",
                Description = "Portafolio de criptomonedas a largo plazo",
                UserId = "user123"
            };

            // Act
            var resultId = await handler.Handle(command, CancellationToken.None);

            // Assert
            resultId.Should().BeGreaterThan(0);

            var created = await context.InvestmentPortfolios.FindAsync(resultId);
            created.Should().NotBeNull();
            created!.Name.Should().Be(command.Name);
            created.Description.Should().Be(command.Description);
            created.UserId.Should().Be(command.UserId);
        }

        [Fact]
        public async Task Handle_Should_Return_Zero_When_Entity_Is_Null()
        { 
            using var context = new InvestmentAppContext(_dbOptions);

            var mockRepo = new FailingPortfolioRepository(context);
            var handler = new CreateInvestmentPortfolioCommandHandler(mockRepo, null!);

            var command = new CreateInvestmentPortfolioCommand
            {
                Name = "Portafolio fallido",
                UserId = "userX"
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }

        private class FailingPortfolioRepository(InvestmentAppContext context) : InvestmentPortfolioRepository(context)
        {
            public override Task<Core.Domain.Entities.InvestmentPortfolio?> AddAsync(Core.Domain.Entities.InvestmentPortfolio entity)
                => Task.FromResult<Core.Domain.Entities.InvestmentPortfolio?>(null);
        }       
    }
}
