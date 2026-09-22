using Microsoft.Extensions.DependencyInjection;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Application.Services;
using zogo.Application.Services.Authentication;

namespace zogo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService,AuthenticationService>();

        services.AddScoped<IPropertyService,PropertyService>();

        services.AddScoped<ISellerService, SellerService>();

        services.AddScoped<IApartmentDetailsService, ApartmentDetailsService>();

        services.AddScoped<IPropertyLegalDetailsService, PropertyLegalDetailsService>();

        services.AddScoped<IPropertyFeatureService, PropertyFeatureService>();

        services.AddScoped<IPropertyAmenityService, PropertyAmenityService>();

        services.AddScoped<IPropertyMediaService, PropertyMediaService>();

        services.AddScoped<ICommonService, CommonService>();

        return services;
    }
}
