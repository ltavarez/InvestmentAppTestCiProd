using AutoMapper;
using FluentAssertions;
using InvestmentApp.Core.Application.Services;
using InvestmentApp.Core.Domain.Interfaces;
using Moq;

namespace InvestmentApp.Unit.Tests.Services
{
    public class DummyEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class DummyDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class GenericServiceTests
    {
        private readonly Mock<IGenericRepository<DummyEntity>> _mockRepo;
        private readonly IMapper _mapper;
        private readonly GenericService<DummyEntity, DummyDto> _service;

        public GenericServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<DummyEntity>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DummyEntity, DummyDto>().ReverseMap();
            });

            _mapper = config.CreateMapper();
            _service = new GenericService<DummyEntity, DummyDto>(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Dto_When_Entity_Added()
        {
            // Arrange
            var dto = new DummyDto { Id = 1, Name = "Test" };
            var entity = new DummyEntity { Id = 1, Name = "Test" };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<DummyEntity>()))
                .ReturnsAsync(entity);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Test");
        }

        [Fact]
        public async Task AddAsync_Should_Return_Null_If_Repository_Returns_Null()
        {
            // Arrange
            var dto = new DummyDto { Id = 1, Name = "NullTest" };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<DummyEntity>()))
                .ReturnsAsync((DummyEntity?)null);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Updated_Dto()
        {
            // Arrange
            var dto = new DummyDto { Id = 2, Name = "Updated" };
            var updatedEntity = new DummyEntity { Id = 2, Name = "Updated" };

            _mockRepo.Setup(r => r.UpdateAsync(2, It.IsAny<DummyEntity>()))
                .ReturnsAsync(updatedEntity);

            // Act
            var result = await _service.UpdateAsync(dto, 2);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Updated");
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Null_If_Not_Found()
        {
            // Arrange
            var dto = new DummyDto { Id = 99, Name = "DoesNotExist" };

            _mockRepo.Setup(r => r.UpdateAsync(99, It.IsAny<DummyEntity>()))
                .ReturnsAsync((DummyEntity?)null);

            // Act
            var result = await _service.UpdateAsync(dto, 99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_True_On_Success()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_On_Exception()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteAsync(1)).Throws(new Exception("DB error"));

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetById_Should_Return_Dto_If_Found()
        {
            // Arrange
            var entity = new DummyEntity { Id = 3, Name = "Found" };
            _mockRepo.Setup(r => r.GetById(3)).ReturnsAsync(entity);

            // Act
            var result = await _service.GetById(3);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Found");
        }

        [Fact]
        public async Task GetById_Should_Return_Null_If_Not_Found()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetById(99)).ReturnsAsync((DummyEntity?)null);

            // Act
            var result = await _service.GetById(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_Should_Return_List_Of_Dtos()
        {
            // Arrange
            var entities = new List<DummyEntity>
            {
                new DummyEntity { Id = 1, Name = "A" },
                new DummyEntity { Id = 2, Name = "B" }
            };

            _mockRepo.Setup(r => r.GetAllList()).ReturnsAsync(entities);

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAll_Should_Return_Empty_List_On_Exception()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllList()).Throws(new Exception("Query failed"));

            // Act
            var result = await _service.GetAll();

            // Assert
            result.Should().BeEmpty();
        }
    }
}
