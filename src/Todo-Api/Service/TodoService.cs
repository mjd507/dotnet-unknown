using dotnet_unknown.Model;

namespace dotnet_unknown.Service;

public class TodoService
{
    private static readonly List<TodoItem> Items = [];
    private static int _nextId = 1;

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await Task.FromResult(Items);
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);
        return await Task.FromResult(item);
    }

    public async Task<TodoItem> CreateAsync(TodoItem item)
    {
        item.Id = _nextId++;
        Items.Add(item);
        return await Task.FromResult(item);
    }
}