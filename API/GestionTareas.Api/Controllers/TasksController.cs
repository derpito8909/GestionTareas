using FluentValidation;
using GestionTareas.Application.Dtos;
using GestionTareas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TaskStatus = GestionTareas.Domain.Enums.TaskStatus;

namespace GestionTareas.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service) => _service = service;

    /// <summary>Crea una tarea nueva.</summary>
    /// <response code="201">Tarea creada.</response>
    /// <response code="400">Datos inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> Create(
        [FromBody] CreateTaskRequest request,
        [FromServices] IValidator<CreateTaskRequest> validator,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var created = await _service.CreateAsync(request, ct);
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Obtiene una tarea por id.</summary>
    /// <response code="200">Tarea encontrada.</response>
    /// <response code="404">No existe.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> GetById(int id, CancellationToken ct)
    {
        var task = await _service.GetByIdAsync(id, ct);
        return Ok(task);
    }

    /// <summary>
    /// Lista tareas con filtros opcionales (incluye filtros por JSON si tu backend los implementa).
    /// </summary>
    /// <response code="200">Listado de tareas.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TaskResponse>), StatusCodes.Status200OK)]
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
            UserId: userId,
            Status: status,
            OrderByCreatedAtDesc: orderByCreatedAtDesc,
            Priority: priority,
            Tag: tag,
            DueDateFrom: dueDateFrom,
            DueDateTo: dueDateTo
        );

        var list = await _service.ListAsync(query, ct);
        return Ok(list);
    }

    /// <summary>Asigna una tarea a un usuario.</summary>
    /// <response code="200">Tarea actualizada.</response>
    /// <response code="400">Datos inválidos.</response>
    /// <response code="404">No existe la tarea o el usuario.</response>
    [HttpPut("{id:int}/assign")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> Assign(
        int id,
        [FromBody] AssignTaskRequest request,
        [FromServices] IValidator<AssignTaskRequest> validator,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var updated = await _service.AssignAsync(id, request, ct);
        return Ok(updated);
    }

    /// <summary>Cambia el estado de una tarea.</summary>
    /// <response code="200">Tarea actualizada.</response>
    /// <response code="400">Transición inválida.</response>
    /// <response code="404">No existe la tarea.</response>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> ChangeStatus(
        int id,
        [FromBody] ChangeTaskStatusRequest request,
        [FromServices] IValidator<ChangeTaskStatusRequest> validator,
        CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(request, ct);

        var updated = await _service.ChangeStatusAsync(id, request, ct);
        return Ok(updated);
    }
}
