using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.AssetType.Commands.DeleteAssetType;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Unit.Tests.Features.AssetType
{
    public class DeleteAssetTypeCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public DeleteAssetTypeCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"InMemoryDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldDeleteAssetType_WhenIdExists()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var existingAssetType = new Core.Domain.Entities.AssetType
            {
                Id = 1,
                Name = "ToDelete",
                Description = "Should be deleted"
            };
            context.AssetTypes.Add(existingAssetType);
            await context.SaveChangesAsync();

            var repository = new AssetTypeRepository(context);
            var handler = new DeleteAssetTypeCommandHandler(repository);
            var command = new DeleteAssetTypeCommand { Id = existingAssetType.Id };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            var deleted = await context.AssetTypes.FindAsync(existingAssetType.Id);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenAssetTypeNotFound()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);
            var handler = new DeleteAssetTypeCommandHandler(repository);
            var command = new DeleteAssetTypeCommand { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Asset type not found with this id");
        }
    }
}
