using HeThongChungCu.Application.Common.Interfaces.Services;

namespace HeThongChungCu.Infrastructure.Email;

public static class DependencyInjection
{
    public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}
