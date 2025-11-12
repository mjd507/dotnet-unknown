using dotnet_unknown.model;

namespace dotnet_unknown.service;

public class TodoService : ITodoService
{
    private static readonly List<TodoItem> _items = [];
    private static int _nextId = 1;

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await Task.FromResult(_items);
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        return await Task.FromResult(item);
    }

    public async Task<TodoItem> CreateAsync(TodoItem item)
    {
        item.Id = _nextId++;
        _items.Add(item);
        return await Task.FromResult(item);
    }

}