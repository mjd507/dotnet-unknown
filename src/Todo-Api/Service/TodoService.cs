using dotnet_unknown.Dao;
using dotnet_unknown.Model;
using Microsoft.EntityFrameworkCore;

namespace dotnet_unknown.Service;

public class TodoService(TodoDao todoDao)
{
    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await todoDao.TodoItems.ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
        return await todoDao.TodoItems.FindAsync(id);
    }

    public async Task<TodoItem> CreateAsync(TodoItem item)
    {
        todoDao.TodoItems.Add(item);
        await todoDao.SaveChangesAsync();
        return item;
    }
}