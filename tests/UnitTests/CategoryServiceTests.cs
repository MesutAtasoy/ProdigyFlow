using Moq;
using ProdigyFlow.Application.Services;
using ProdigyFlow.Application.Services.Category;
using ProdigyFlow.Domain.Entities;
using ProdigyFlow.Domain.Entities.Categories;
using ProdigyFlow.Domain.Repositories;

namespace UnitTests;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockRepo;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepo = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Name = "Electronics" },
            new Category { Name = "Books" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllCategoriesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Name == "Electronics");
        Assert.Contains(result, c => c.Name == "Books");
    }

    [Fact]
    public async Task GetCategory_ReturnsCategory_WhenExists()
    {
        // Arrange
        var category = new Category { Id = Guid.NewGuid(), Name = "Toys" };
        _mockRepo.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

        // Act
        var result = await _service.GetCategoryAsync(category.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Toys", result!.Name);
    }

    [Fact]
    public async Task GetCategory_ReturnsNull_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);

        // Act
        var result = await _service.GetCategoryAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddCategory_CallsRepository()
    {
        // Arrange
        var category = new Category { Name = "Sports" };

        // Act
        await _service.AddCategoryAsync(category);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(category), Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_CallsRepository()
    {
        // Arrange
        var category = new Category { Id = Guid.NewGuid(), Name = "Home" };

        // Act
        await _service.UpdateCategoryAsync(category);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(category), Times.Once);
    }

    
    [Fact]
    public async Task DeleteCategoryAsync_CallsRepository_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Test Category" };

        _mockRepo.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _mockRepo.Setup(r => r.DeleteAsync(categoryId))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteCategoryAsync(categoryId);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_DoesNotCallRepository_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _mockRepo.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        await _service.DeleteCategoryAsync(categoryId);

        // Assert
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}