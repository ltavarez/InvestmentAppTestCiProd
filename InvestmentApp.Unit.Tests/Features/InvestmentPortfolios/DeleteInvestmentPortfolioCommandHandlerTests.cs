using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.DeleteInvestmentPortfolio;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentPortfolios
{
    public class DeleteInvestmentPortfolioCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public DeleteInvestmentPortfolioCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"Db_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_Delete_Portfolio_When_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var portfolio = new InvestmentPortfolio
            {
                Id = 1,
                Name = "Portafolio de prueba",
                UserId = "user1",
                Description = "Para eliminar"
            };

            context.InvestmentPortfolios.Add(portfolio);
            await context.SaveChangesAsync();

            var repository = new InvestmentPortfolioRepository(context);
            var handler = new DeleteInvestmentPortfolioCommandHandler(repository, null!);

            var command = new DeleteInvestmentPortfolioCommand { Id = portfolio.Id };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var deleted = await context.InvestmentPortfolios.FindAsync(portfolio.Id);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Portfolio_Does_Not_Exist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var repository = new InvestmentPortfolioRepository(context);
            var handler = new DeleteInvestmentPortfolioCommandHandler(repository, null!);

            var command = new DeleteInvestmentPortfolioCommand { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Investment Portfolio not found with this id");
        }
    }
}
