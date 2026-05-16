using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public ActionResult<List<TodoItem>> GetAll()
    {
        return Ok(_todoService.GetAll());
    }

    [HttpPost]
    public ActionResult<TodoItem> Add([FromBody] CreateTodoRequest request)
    {
        var todo = _todoService.Add(request.Title);

        return Ok(todo);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var success = _todoService.Delete(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}

public class CreateTodoRequest
{
    public string Title { get; set; } = string.Empty;
}