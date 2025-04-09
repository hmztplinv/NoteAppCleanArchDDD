using Microsoft.Extensions.DependencyInjection;
using NoteApp.Application.Interfaces.Services;
using NoteApp.Infrastructure.Services;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();


        return services;
    }
}