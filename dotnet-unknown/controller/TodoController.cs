using Microsoft.AspNetCore.Mvc;
using dotnet_unknown.model;
using dotnet_unknown.service;

namespace dotnet_unknown.controller;

[Route("api/[controller]")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
    {
        return Ok(await _todoService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetById(int id)
    {
        return Ok(await _todoService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create(TodoItem item)
    {
        var created = await _todoService.CreateAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

}