using GestionTareas.Domain.Errors;

namespace GestionTareas.Api.Middlewares;

public static class ErrorCatalog
{
    private static readonly Dictionary<string, string> Map = new()
    {
        [ErrorCodes.UserNotFound] = "El usuario no existe.",
        [ErrorCodes.UserEmailRequired] = "El correo es obligatorio.",
        [ErrorCodes.UserEmailDuplicate] = "Ya existe un usuario con ese correo.",
        [ErrorCodes.UserEmailMaxLongitude] = "El correo no puede superar 255 caracteres.",
        [ErrorCodes.UserNameRequired] = "El nombre del usuario es obligatorio.",
        [ErrorCodes.UserNameMaxLongitude] = "El nombre no puede superar 100 caracteres.",
        

        [ErrorCodes.TaskNotFound] = "La tarea no existe.",
        [ErrorCodes.TaskTitleRequired] = "El título es obligatorio.",
        [ErrorCodes.TaskAssignedUserRequired] = "Debes asignar la tarea a un usuario.",
        [ErrorCodes.TaskInvalidTransition] = "No puedes pasar una tarea de Pendiente a Finalizada directamente. Primero cámbiala a En Progreso.",
        [ErrorCodes.TaskAdditionalInfoInvalidJson] = "La información adicional debe ser un JSON válido.",

        [ErrorCodes.ValidationFailed] = "Hay errores de validación.",
        [ErrorCodes.UnexpectedError] = "Ocurrió un error inesperado. Intenta de nuevo."
    };

    public static string Message(string code)
        => Map.TryGetValue(code, out var msg) ? msg : Map[ErrorCodes.UnexpectedError];
}