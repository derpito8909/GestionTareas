using TaskStatus = GestionTareas.Domain.Enums.TaskStatus;
using GestionTareas.Domain.Errors;
using GestionTareas.Domain.Exceptions;

namespace GestionTareas.Domain.Entities;

public class TaskItem
{
    public int Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; } = TaskStatus.Pending;

    public int AssignedUserId { get; private set; }
    public User AssignedUser { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; } 
    public string? AdditionalInfoJson { get; private set; } 

    private TaskItem() { } 

    public TaskItem(string title, string? description, int assignedUserId, string? additionalInfoJson)
    {
        SetTitle(title);
        SetDescription(description);
        AssignTo(assignedUserId);
        
        AdditionalInfoJson = NormalizeNullable(additionalInfoJson);

        Status = TaskStatus.Pending;
    }

    public void AssignTo(int userId)
    {
        if (userId <= 0)
            throw new BusinessRuleAppException(ErrorCodes.TaskAssignedUserRequired);

        AssignedUserId = userId;
    }

    public void ChangeStatus(TaskStatus newStatus)
    {
        if (newStatus == Status) return;
        
        if (Status == TaskStatus.Pending && newStatus == TaskStatus.Done)
            throw new BusinessRuleAppException(ErrorCodes.TaskInvalidTransition);

        Status = newStatus;
    }

    public void UpdateAdditionalInfoJson(string? json)
    {
        AdditionalInfoJson = NormalizeNullable(json);
    }

    private void SetTitle(string title)
    {
        title = (title ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessRuleAppException(ErrorCodes.TaskTitleRequired);

        if (title.Length > 200)
            throw new BusinessRuleAppException(ErrorCodes.TaskTitleRequired);

        Title = title;
    }

    private void SetDescription(string? description)
    {
        description = NormalizeNullable(description);

        if (description is not null && description.Length > 1000)
            throw new BusinessRuleAppException(ErrorCodes.ValidationFailed);

        Description = description;
    }

    private static string? NormalizeNullable(string? value)
    {
        value = value?.Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
