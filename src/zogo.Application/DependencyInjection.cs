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
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped<IPropertyService, PropertyService>();

        services.AddScoped<ISellerService, SellerService>();

        services.AddScoped<IApartmentDetailsService, ApartmentDetailsService>();

        services.AddScoped<IPropertyLegalDetailsService, PropertyLegalDetailsService>();
        services.AddScoped<IPropertyFinancialsService, PropertyFinancialsService>();

        services.AddScoped<IPropertyFeatureService, PropertyFeatureService>();

        services.AddScoped<IPropertyAmenityService, PropertyAmenityService>();

        services.AddScoped<IPropertyMediaService, PropertyMediaService>();
        services.AddScoped<IPropertyDocumentService, PropertyDocumentService>();

        services.AddScoped<ICommonService, CommonService>();

        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IPropertyInterestService, PropertyInterestService>();
        services.AddScoped<IBuyerPropertyInteractionService, BuyerPropertyInteractionService>();
        services.AddScoped<IBuyerPropertyService, BuyerPropertyService>();

        services.AddScoped<IAdminAgentService, AdminAgentService>();
        services.AddScoped<IAdminPropertyService, AdminPropertyService>();
        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<IAgentNotificationService, AgentNotificationService>();
        services.AddScoped<IPropertyVisitService, PropertyVisitService>();
        services.AddScoped<IPropertyOfferService, PropertyOfferService>();
        services.AddScoped<IPropertyTransactionService, PropertyTransactionService>();
        services.AddScoped<IUserNotificationService, UserNotificationService>();

        return services;
    }
}
