using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    List<TodoItem> GetAll();

    TodoItem Add(string title);

    bool Delete(Guid id);
}