using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService : ITodoService
{
    private readonly List<TodoItem> _todos = [];

    public List<TodoItem> GetAll()
    {
        return _todos;
    }

    public TodoItem Add(string title)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = title
        };

        _todos.Add(todo);

        return todo;
    }

    public bool Delete(Guid id)
    {
        var todo = _todos.FirstOrDefault(x => x.Id == id);

        if (todo is null)
        {
            return false;
        }

        _todos.Remove(todo);

        return true;
    }
}