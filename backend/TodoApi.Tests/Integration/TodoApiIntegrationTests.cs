using Microsoft.AspNetCore.Mvc.Testing;
using TodoApi.Models;
using System.Net;
using System.Text.Json;
using Xunit;
using TodoApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace TodoApi.Tests.Integration;

public class IsolatedIntegrationTestBase : IAsyncLifetime
{
    protected HttpClient _client = null!;
    protected WebApplicationFactory<Program> _factory = null!;

    public virtual async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the singleton TodoService
                    var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITodoService));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add a new instance for each test
                    services.AddSingleton<ITodoService, TodoService>();
                });
            });
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        _client?.Dispose();
        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }
        await Task.CompletedTask;
    }
}

public class TodoApiIntegrationTests_GetTodos : IsolatedIntegrationTestBase
{
    [Fact]
    public async Task GetTodos_ReturnsSuccessAndEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var todos = JsonSerializer.Deserialize<List<TodoItem>>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        Assert.NotNull(todos);
        Assert.Empty(todos);
    }
}

public class TodoApiIntegrationTests_AddTodo : IsolatedIntegrationTestBase
{
    [Fact]
    public async Task AddTodo_WithValidTitle_ReturnsCreatedTodo()
    {
        // Arrange
        var request = new { title = "Integration Test Todo" };
        var content = new StringContent(
            JsonSerializer.Serialize(request),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        // Act
        var response = await _client.PostAsync("/api/todos", content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var todo = JsonSerializer.Deserialize<TodoItem>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        Assert.NotNull(todo);
        Assert.Equal("Integration Test Todo", todo.Title);
        Assert.NotEqual(Guid.Empty, todo.Id);
    }
}

public class TodoApiIntegrationTests_AddAndGet : IsolatedIntegrationTestBase
{
    [Fact]
    public async Task AddTodo_And_GetTodos_ReturnsAddedTodo()
    {
        // Arrange
        var request = new { title = "New Todo" };
        var content = new StringContent(
            JsonSerializer.Serialize(request),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        // Act
        var addResponse = await _client.PostAsync("/api/todos", content);
        var addContent = await addResponse.Content.ReadAsStringAsync();
        var addedTodo = JsonSerializer.Deserialize<TodoItem>(
            addContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        var getResponse = await _client.GetAsync("/api/todos");
        var getContent = await getResponse.Content.ReadAsStringAsync();
        var todos = JsonSerializer.Deserialize<List<TodoItem>>(
            getContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        // Assert
        Assert.NotNull(addedTodo);
        Assert.NotNull(todos);
        Assert.Single(todos);
        Assert.Equal(addedTodo.Id, todos[0].Id);
    }
}

public class TodoApiIntegrationTests_Delete : IsolatedIntegrationTestBase
{
    [Fact]
    public async Task DeleteTodo_WithExistingId_ReturnsNoContent()
    {
        // Arrange
        var addRequest = new { title = "Todo to Delete" };
        var addContent = new StringContent(
            JsonSerializer.Serialize(addRequest),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        var addResponse = await _client.PostAsync("/api/todos", addContent);
        var addResponseContent = await addResponse.Content.ReadAsStringAsync();
        var todo = JsonSerializer.Deserialize<TodoItem>(
            addResponseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        Assert.NotNull(todo);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/todos/{todo.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var randomId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/todos/{randomId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

public class TodoApiIntegrationTests_Workflow : IsolatedIntegrationTestBase
{
    [Fact]
    public async Task FullWorkflow_AddMultipleTodos_DeleteOne_VerifyRemoved()
    {
        // Arrange
        var titles = new[] { "First Todo", "Second Todo", "Third Todo" };
        var createdIds = new List<Guid>();

        // Add todos
        foreach (var title in titles)
        {
            var request = new { title };
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _client.PostAsync("/api/todos", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var todo = JsonSerializer.Deserialize<TodoItem>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(todo);
            createdIds.Add(todo.Id);
        }

        // Act - Delete the second todo
        var deleteResponse = await _client.DeleteAsync($"/api/todos/{createdIds[1]}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify todos count
        var getResponse = await _client.GetAsync("/api/todos");
        var getContent = await getResponse.Content.ReadAsStringAsync();
        var remainingTodos = JsonSerializer.Deserialize<List<TodoItem>>(
            getContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        Assert.NotNull(remainingTodos);
        Assert.Equal(2, remainingTodos.Count);
        Assert.DoesNotContain(remainingTodos, t => t.Id == createdIds[1]);
    }
}
