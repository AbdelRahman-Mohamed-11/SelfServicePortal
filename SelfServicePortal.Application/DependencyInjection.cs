using Microsoft.Extensions.DependencyInjection;
using SelfServicePortal.Application.Services;
using SelfServicePortal.Core.Interfaces;

namespace SelfServicePortal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IIncidentService, IncidentService>();

        return services;
    }
}

