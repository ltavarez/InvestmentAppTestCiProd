using FluentAssertions;
using InvestmentApp.Core.Application.Exceptions;
using InvestmentApp.Core.Application.Features.AssetType.Commands.CreateAssetType;
using InvestmentApp.Core.Domain.Interfaces;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InvestmentApp.Unit.Tests.Features.AssetType
{
    public class CreateAssetTypeCommandHandlerTests
    {
        private readonly DbContextOptions<InvestmentAppContext> _dbOptions;

        public CreateAssetTypeCommandHandlerTests()
        {            
            _dbOptions = new DbContextOptionsBuilder<InvestmentAppContext>()
                .UseInMemoryDatabase(databaseName: $"InMemoryDb_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldReturnAssetTypeId_WhenCreationIsSuccessful()
        {
            // Arrange
            using var context = new InvestmentAppContext(_dbOptions);
            var repository = new AssetTypeRepository(context);
            var handler = new CreateAssetTypeCommandHandler(repository);

            var command = new CreateAssetTypeCommand
            {
                Name = "Crypto",
                Description = "Cryptocurrency type"
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
            var createdEntity = await context.AssetTypes.FindAsync(result);
            createdEntity.Should().NotBeNull();
            createdEntity!.Name.Should().Be(command.Name);
            createdEntity.Description.Should().Be(command.Description);
        }

        [Fact]
        public async Task Handle_ShouldReturnZero_WhenRepositoryReturnsNull()
        {
            // Arrange
            Mock<IAssetTypeRepository> _mockRepository = new();
            CreateAssetTypeCommandHandler _handler = new(_mockRepository.Object);

            var command = new CreateAssetTypeCommand
            {
                Name = "Real Estate",
                Description = "Property-based investment"
            };

            _mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Core.Domain.Entities.AssetType>()))
                .ReturnsAsync((Core.Domain.Entities.AssetType?)null);

            // Act && Assert
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>().WithMessage("Error created assets type");
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Core.Domain.Entities.AssetType>()), Times.Once);          
        }  
    }
}
