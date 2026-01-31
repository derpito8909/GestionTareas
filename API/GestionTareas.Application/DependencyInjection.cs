using FluentValidation;
using GestionTareas.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using GestionTareas.Application.Services;
using GestionTareas.Application.Validation.Users;
using GestionTareas.Application.Validation.Task;

namespace GestionTareas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UsersService>();
        services.AddScoped<ITaskService, TaskService>();
        
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();

        return services;
    }
}