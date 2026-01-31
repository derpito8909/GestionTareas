namespace GestionTareas.Domain.Errors;

public static class ErrorCodes
{
    public const string UserNotFound = "user.not_found";
    public const string UserEmailRequired = "user.email.required";
    public const string UserEmailDuplicate = "user.email.duplicate";
    public const string UserEmailInvalid = "user.email.invalid";
    public const string UserEmailMaxLongitude = "user.email.longitude";
    public const string UserNameRequired = "user.name.requeired";
    public const string UserNameMaxLongitude = "user.name.longitude";
    
    

    public const string TaskNotFound = "task.not_found";
    public const string TaskTitleRequired = "task.title.required";
    public const string TaskAssignedUserRequired = "task.assigned_user.required";
    public const string TaskInvalidTransition = "task.status.invalid_transition";
    public const string TaskAdditionalInfoInvalidJson = "task.additional_info.invalid_json";

    public const string ValidationFailed = "validation.failed";
    public const string UnexpectedError = "unexpected_error";
}