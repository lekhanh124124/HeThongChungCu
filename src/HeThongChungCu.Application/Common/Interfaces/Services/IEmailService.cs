namespace HeThongChungCu.Application.Common.Interfaces.Services;

public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);

    Task SendTemplateAsync(
        string to,
        string templateId,
        object templateData,
        CancellationToken cancellationToken = default);

    Task SendWelcomeEmailAsync(
        string to,
        string userName,
        CancellationToken cancellationToken = default);

    Task SendPasswordResetEmailAsync(
        string to,
        string resetCode,
        CancellationToken cancellationToken = default);

    Task SendAssessmentReadyEmailAsync(
        string to,
        string userName,
        string assessmentName,
        CancellationToken cancellationToken = default);

    Task SendIdentificationEmailAsync(
        string to,
        string identificationLink,
        CancellationToken cancellationToken = default);

    Task SendStaffWelcomeEmailAsync(
        string to,
        string fullName,
        string userName,
        string password,
        CancellationToken cancellationToken = default);

    string MaskEmail(string email);
}
