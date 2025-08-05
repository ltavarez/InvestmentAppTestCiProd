using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.InvestmentPortfolios.Commands.UpdateInvestmentPortfolio;
using InvestmentApp.Core.Domain.Entities;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.InvestmentPortfolios
{
    public class UpdateInvestmentPortfolioCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public UpdateInvestmentPortfolioCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase($"Db_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_Update_Portfolio_When_Exists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var original = new InvestmentPortfolio
            {
                Id = 1,
                Name = "Original Portfolio",
                Description = "Old description",
                UserId = "user1"
            };

            context.InvestmentPortfolios.Add(original);
            await context.SaveChangesAsync();

            var repository = new InvestmentPortfolioRepository(context);
            var handler = new UpdateInvestmentPortfolioCommandHandler(repository, null!);

            var command = new UpdateInvestmentPortfolioCommand
            {
                Id = original.Id,
                Name = "Updated Portfolio",
                Description = "New description",
                UserId = "user1"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);

            var updated = await context.InvestmentPortfolios.FindAsync(original.Id);
            updated.Should().NotBeNull();
            updated!.Name.Should().Be("Updated Portfolio");
            updated.Description.Should().Be("New description");
            updated.UserId.Should().Be("user1");
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Portfolio_Does_Not_Exist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);

            var repository = new InvestmentPortfolioRepository(context);
            var handler = new UpdateInvestmentPortfolioCommandHandler(repository, null!);

            var command = new UpdateInvestmentPortfolioCommand
            {
                Id = 999,
                Name = "Nonexistent",
                Description = "None",
                UserId = "user1"
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Investment Portfolio not found with this id");
        }
    }
}
