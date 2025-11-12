using dotnet_unknown.service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

app.MapControllers();

app.Run();