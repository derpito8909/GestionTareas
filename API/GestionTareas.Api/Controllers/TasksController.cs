using GestionTareas.Application.Dtos;
using GestionTareas.Application.Interfaces;
using TaskStatus = GestionTareas.Domain.Enums.TaskStatus;
using Microsoft.AspNetCore.Mvc;

namespace GestionTareas.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service) => _service = service;

    /// <summary>
    // /// Crea una tarea nueva.
    // /// </summary>
    // /// <param name="request">Datos de la tarea.</param>
    // /// <param name="ct">Token de cancelación.</param>
    /// <returns>201 con el id creado.</returns>
    /// <response code="201">usuario creado.</response>
    /// <response code="400">Datos inválidos.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> Create([FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    /// <summary>
    // /// Lista Tareas</c>.
    // /// </summary>
    // /// <returns>Listado de personas ordenadas por fecha de creación descendente.</returns>
    // /// <response code="200">Retorna el listado.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TaskResponse>>> GetAll(
        [FromQuery] int? userId,
        [FromQuery] TaskStatus? status,
        [FromQuery] bool orderByCreatedAtDesc = true,
        [FromQuery] string? priority = null,
        [FromQuery] string? tag = null,
        [FromQuery] DateTime? dueDateFrom = null,
        [FromQuery] DateTime? dueDateTo = null,
        CancellationToken ct = default)
    {
        var query = new TaskQuery(
            userId,
            status,
            orderByCreatedAtDesc,
            priority,
            tag,
            dueDateFrom,
            dueDateTo
        );
        return Ok(await _service.ListAsync(query, ct));
    }

    
    [HttpPut("{id:int}/assign")]
    public async Task<ActionResult<TaskResponse>> Assign(int id, [FromBody] AssignTaskRequest request, CancellationToken ct)
        => Ok(await _service.AssignAsync(id, request, ct));

    // PUT /api/tasks/{id}/status
    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<TaskResponse>> ChangeStatus(int id, [FromBody] ChangeTaskStatusRequest request, CancellationToken ct)
        => Ok(await _service.ChangeStatusAsync(id, request, ct));
}