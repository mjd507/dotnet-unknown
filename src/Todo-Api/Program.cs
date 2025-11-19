using dotnet_unknown.Dao;
using dotnet_unknown.Exceptions;
using dotnet_unknown.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<TodoService>();

builder.Services.AddDbContext<TodoDao>(options => options.UseSqlite("Data Source=Todo.db"));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Ensure Database and tables are created.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TodoDao>();
    context.Database.EnsureCreated();
}

app.MapControllers();

app.UseExceptionHandler();

app.Run();