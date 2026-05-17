using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests.Controllers;

public class TodosControllerTests
{
    [Fact]
    public void GetAll_ReturnsOkWithTodos()
    {
        // Arrange
        var mockTodoService = new Mock<ITodoService>();
        var todos = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Test 1" },
            new TodoItem { Id = Guid.NewGuid(), Title = "Test 2" }
        };
        mockTodoService.Setup(s => s.GetAll()).Returns(todos);
        var controller = new TodosController(mockTodoService.Object);

        // Act
        var result = controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTodos = Assert.IsType<List<TodoItem>>(okResult.Value);
        Assert.Equal(2, returnedTodos.Count);
        mockTodoService.Verify(s => s.GetAll(), Times.Once);
    }

    [Fact]
    public void GetAll_WhenNoTodosExist_ReturnsEmptyList()
    {
        // Arrange
        var mockTodoService = new Mock<ITodoService>();
        mockTodoService.Setup(s => s.GetAll()).Returns(new List<TodoItem>());
        var controller = new TodosController(mockTodoService.Object);

        // Act
        var result = controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTodos = Assert.IsType<List<TodoItem>>(okResult.Value);
        Assert.Empty(returnedTodos);
    }

    [Fact]
    public void Add_WithValidRequest_ReturnsOkWithTodo()
    {
        // Arrange
        var mockTodoService = new Mock<ITodoService>();
        var newTodo = new TodoItem { Id = Guid.NewGuid(), Title = "New Todo" };
        mockTodoService.Setup(s => s.Add(It.IsAny<string>())).Returns(newTodo);
        var controller = new TodosController(mockTodoService.Object);
        var request = new CreateTodoRequest { Title = "New Todo" };

        // Act
        var result = controller.Add(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTodo = Assert.IsType<TodoItem>(okResult.Value);
        Assert.Equal(newTodo.Id, returnedTodo.Id);
        Assert.Equal(newTodo.Title, returnedTodo.Title);
        mockTodoService.Verify(s => s.Add("New Todo"), Times.Once);
    }

    [Fact]
    public void Add_CallsTodoServiceWithCorrectTitle()
    {
        // Arrange
        var mockTodoService = new Mock<ITodoService>();
        var newTodo = new TodoItem { Id = Guid.NewGuid(), Title = "Test Title" };
        mockTodoService.Setup(s => s.Add(It.IsAny<string>())).Returns(newTodo);
        var controller = new TodosController(mockTodoService.Object);
        var request = new CreateTodoRequest { Title = "Test Title" };

        // Act
        controller.Add(request);

        // Assert
        mockTodoService.Verify(s => s.Add("Test Title"), Times.Once);
    }

    [Fact]
    public void Delete_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var mockTodoService = new Mock<ITodoService>();
        var todoId = Guid.NewGuid();
        mockTodoService.Setup(s => s.Delete(todoId)).Returns(true);
        var controller = new TodosController(mockTodoService.Object);

        // Act
        var result = controller.Delete(todoId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        mockTodoService.Verify(s => s.Delete(todoId), Times.Once);
    }

    [Fact]
    public void Delete_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var mockTodoService = new Mock<ITodoService>();
        var todoId = Guid.NewGuid();
        mockTodoService.Setup(s => s.Delete(todoId)).Returns(false);
        var controller = new TodosController(mockTodoService.Object);

        // Act
        var result = controller.Delete(todoId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        mockTodoService.Verify(s => s.Delete(todoId), Times.Once);
    }
}
