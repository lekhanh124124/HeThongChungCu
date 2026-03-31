using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Infrastructure.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace HeThongChungCu.Infrastructure.Notifications;

public static class DependencyInjection
{
    public static IServiceCollection AddNotification(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddScoped<INotificationService, SignalRNotificationService>();
        
        return services;
    }
}
