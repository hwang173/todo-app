using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests.Services;

public class TodoServiceTests
{
    [Fact]
    public void GetAll_WhenNoTodosExist_ReturnsEmptyList()
    {
        // Arrange
        var service = new TodoService();

        // Act
        var result = service.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Add_WithValidTitle_ReturnsTodoItem()
    {
        // Arrange
        var service = new TodoService();
        var title = "Test Todo";

        // Act
        var result = service.Add(title);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(title, result.Title);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public void Add_WithValidTitle_AddsTodoToList()
    {
        // Arrange
        var service = new TodoService();
        var title = "Test Todo";

        // Act
        service.Add(title);
        var todos = service.GetAll();

        // Assert
        Assert.Single(todos);
        Assert.Equal(title, todos[0].Title);
    }

    [Fact]
    public void Add_WithMultipleTitles_AddsMultipleTodos()
    {
        // Arrange
        var service = new TodoService();

        // Act
        service.Add("Todo 1");
        service.Add("Todo 2");
        service.Add("Todo 3");
        var todos = service.GetAll();

        // Assert
        Assert.Equal(3, todos.Count);
    }

    [Fact]
    public void Delete_WithExistingId_RemovesTodo()
    {
        // Arrange
        var service = new TodoService();
        var todo = service.Add("Test Todo");

        // Act
        var result = service.Delete(todo.Id);

        // Assert
        Assert.True(result);
        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void Delete_WithNonExistingId_ReturnsFalse()
    {
        // Arrange
        var service = new TodoService();
        var randomId = Guid.NewGuid();

        // Act
        var result = service.Delete(randomId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Delete_WithNonExistingId_DoesNotAffectOtherTodos()
    {
        // Arrange
        var service = new TodoService();
        service.Add("Todo 1");
        service.Add("Todo 2");
        var randomId = Guid.NewGuid();

        // Act
        service.Delete(randomId);

        // Assert
        Assert.Equal(2, service.GetAll().Count);
    }

    [Fact]
    public void Delete_RemovesOnlyTargetedTodo()
    {
        // Arrange
        var service = new TodoService();
        var todo1 = service.Add("Todo 1");
        var todo2 = service.Add("Todo 2");

        // Act
        service.Delete(todo1.Id);

        // Assert
        var todos = service.GetAll();
        Assert.Single(todos);
        Assert.Equal(todo2.Id, todos[0].Id);
    }
}
