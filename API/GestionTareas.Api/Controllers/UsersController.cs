using GestionTareas.Application.Dtos;
using GestionTareas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionTareas.Api.Controllers;
/// <summary>
/// Endpoints para gestión de usuarios 
/// </summary>
/// <remarks>
/// Este controlador registra usuarios en la tabla <c>dbo.Users</c>.
/// </remarks>
[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service) => _service = service;

    /// <summary>
    // /// Registra un usuario nuevo.
    // /// </summary>
    // /// <param name="request">Datos de registro: usuario y contraseña.</param>
    // /// <param name="ct">Token de cancelación del request.</param>
    // /// <returns>201 con el id del usuario creado.</returns>
    // /// <response code="201">Usuario creado correctamente.</response>
    // /// <response code="400">Datos inválidos (validación de campos).</response>s
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var created = await _service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    /// <summary>
    // /// Lista de usuarios</c>.
    // /// </summary>
    // /// <returns>Listado de usuarios por fecha de creación descendente.</returns>
    // /// <response code="200">Retorna el listado.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken ct)
        => Ok(await _service.ListAsync(ct));
}