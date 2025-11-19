using dotnet_unknown.Model;
using Microsoft.EntityFrameworkCore;

namespace dotnet_unknown.Dao;

public class TodoDao(DbContextOptions<TodoDao> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}