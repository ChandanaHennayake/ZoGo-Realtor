using Microsoft.Extensions.DependencyInjection;
using zogo.Application.Interfaces.Services;
using zogo.Application.Services.Authentication;

namespace zogo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<
            IAuthenticationService,
            AuthenticationService>();

        return services;
    }
}