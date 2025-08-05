using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.AssetType.Commands.UpdateAssetType;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.AssetType
{
    public class UpdateAssetTypeCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public UpdateAssetTypeCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"InMemoryDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_Should_UpdateAssetType_When_AssetTypeExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var existingAssetType = new Core.Domain.Entities.AssetType
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Description"
            };
            context.AssetTypes.Add(existingAssetType);
            await context.SaveChangesAsync();

            var repository = new AssetTypeRepository(context);
            var handler = new UpdateAssetTypeCommandHandler(repository);

            var command = new UpdateAssetTypeCommand
            {
                Id = 1,
                Name = "Updated Name",
                Description = "Updated Description"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var updated = await context.AssetTypes.FindAsync(command.Id);
            updated.Should().NotBeNull();
            updated!.Name.Should().Be(command.Name);
            updated.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_AssetTypeDoesNotExist()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);
            var handler = new UpdateAssetTypeCommandHandler(repository);

            var command = new UpdateAssetTypeCommand
            {
                Id = 999,
                Name = "New Name",
                Description = "New Description"
            };

            // Act
            Func<Task> act = () => handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Asset type not found with this id");
        }
    }
}
