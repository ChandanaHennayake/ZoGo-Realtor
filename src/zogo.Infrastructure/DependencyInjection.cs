using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Application.Services;
using zogo.Infrastructure.Auth;
using zogo.Infrastructure.Persistence;
using zogo.Infrastructure.Persistence.Repositories;
using zogo.Infrastructure.Persistence.Repositories.Identity;
using zogo.Infrastructure.Storage;

namespace zogo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>options.UseNpgsql( configuration.GetConnectionString( "DefaultConnection")));
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IGoogleAuthenticationService,GoogleAuthenticationService>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IApartmentDetailsRepository, ApartmentDetailsRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IPropertyFinancialsRepository, PropertyFinancialsRepository>();
        services.AddScoped<IPropertyLegalDetailsRepository,PropertyLegalDetailsRepository>();
        services.AddScoped<IPropertyFeatureRepository, PropertyFeatureRepository>();
        services.AddScoped<IPropertyAmenityRepository, PropertyAmenityRepository>();
        services.AddScoped<IFileStorageService, CloudflareR2StorageService>();


        return services;
    }
}