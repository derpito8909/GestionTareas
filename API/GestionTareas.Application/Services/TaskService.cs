using FluentValidation;
using GestionTareas.Application.Dtos;
using GestionTareas.Application.Interfaces;
using GestionTareas.Domain.Entities;
using GestionTareas.Domain.Errors;
using GestionTareas.Domain.Exceptions;

namespace GestionTareas.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _tasks;
    private readonly IUserRepository _users;

    private readonly IValidator<CreateTaskRequest> _createValidator;
    private readonly IValidator<AssignTaskRequest> _assignValidator;
    private readonly IValidator<ChangeTaskStatusRequest> _statusValidator;

    public TaskService(
        ITaskRepository tasks,
        IUserRepository users,
        IValidator<CreateTaskRequest> createValidator,
        IValidator<AssignTaskRequest> assignValidator,
        IValidator<ChangeTaskStatusRequest> statusValidator)
    {
        _tasks = tasks;
        _users = users;
        _createValidator = createValidator;
        _assignValidator = assignValidator;
        _statusValidator = statusValidator;
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        if (!await _users.ExistsAsync(request.AssignedUserId, ct))
            throw new NotFoundException(ErrorCodes.UserNotFound);

        var entity = new TaskItem(
            request.Title,
            request.Description,
            request.AssignedUserId,
            request.AdditionalInfoJson
        );

        entity = await _tasks.AddAsync(entity, ct);
        await _tasks.SaveChangesAsync(ct);
        
        var assignedUser = await _users.GetByIdAsync(entity.AssignedUserId, ct);
        if (assignedUser is null) throw new NotFoundException(ErrorCodes.UserNotFound);

        return Map(entity, assignedUser.Name);
    }

    public async Task<IReadOnlyList<TaskResponse>> ListAsync(TaskQuery query, CancellationToken ct)
    {
        var tasks = await _tasks.ListAsync(query, ct);
        
        return tasks.Select(t => Map(t, t.AssignedUser.Name)).ToList();
    }

    public async Task<TaskResponse> AssignAsync(int taskId, AssignTaskRequest request, CancellationToken ct)
    {
        await _assignValidator.ValidateAndThrowAsync(request, ct);

        var task = await _tasks.GetByIdAsync(taskId, ct);
        if (task is null) throw new NotFoundException(ErrorCodes.TaskNotFound);

        if (!await _users.ExistsAsync(request.UserId, ct))
            throw new NotFoundException(ErrorCodes.UserNotFound);

        task.AssignTo(request.UserId);
        await _tasks.SaveChangesAsync(ct);

        var assignedUser = await _users.GetByIdAsync(task.AssignedUserId, ct);
        if (assignedUser is null) throw new NotFoundException(ErrorCodes.UserNotFound);

        return Map(task, assignedUser.Name);
    }

    public async Task<TaskResponse> ChangeStatusAsync(int taskId, ChangeTaskStatusRequest request, CancellationToken ct)
    {
        await _statusValidator.ValidateAndThrowAsync(request, ct);

        var task = await _tasks.GetByIdAsync(taskId, ct);
        if (task is null) throw new NotFoundException(ErrorCodes.TaskNotFound);
        
        task.ChangeStatus(request.Status);

        await _tasks.SaveChangesAsync(ct);

        return Map(task, task.AssignedUser.Name);
    }

    private static TaskResponse Map(TaskItem t, string userName)
        => new(
            t.Id,
            t.Title,
            t.Description,
            t.Status,
            t.CreatedAt,
            t.AssignedUserId,
            userName,
            t.AdditionalInfoJson
        );
}