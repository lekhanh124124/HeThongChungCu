using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Infrastructure.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeThongChungCu.Infrastructure.Email;

public static class DependencyInjection
{
    public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}
