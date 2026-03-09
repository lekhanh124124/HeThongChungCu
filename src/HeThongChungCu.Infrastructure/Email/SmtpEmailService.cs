using HeThongChungCu.Application.Common.Interfaces.Services;
using HeThongChungCu.Application.Common.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace HeThongChungCu.Infrastructure.Email;

internal sealed class SmtpEmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<EmailOptions> options,
        ILogger<SmtpEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        if (!_options.EnableSending)
        {
            _logger.LogInformation("Email sending disabled. Would send to {To}: {Subject}", to, subject);
            return;
        }

        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_options.DisplayName, _options.Mail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls, cancellationToken);
            await smtp.AuthenticateAsync(_options.Mail, _options.Password, cancellationToken);
            await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent to {To}: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}. Subject: {Subject}", to, subject);
            throw new EmailSendException($"Failed to send email: {ex.Message}", ex);
        }
    }

    public Task SendTemplateAsync(
        string to,
        string templateId,
        object templateData,
        CancellationToken cancellationToken = default)
    {
        // For SMTP, template resolution needs to happen before sending.
        // A full implementation would use a template engine like RazorLight or Fluid.
        // For now, we simulate sending the template.
        throw new NotImplementedException("Template sending is not fully implemented for basic SMTP provider without a template engine.");
    }

    public async Task SendWelcomeEmailAsync(
        string to,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var subject = "Chào mừng đến với hệ thống!";
        var body = $"<h1>Xin chào {userName}</h1><p>Cảm ơn bạn đã đăng ký.</p>";
        await SendAsync(to, subject, body, cancellationToken);
    }

    public async Task SendPasswordResetEmailAsync(
        string to,
        string resetCode,
        CancellationToken cancellationToken = default)
    {
        var subject = "Yêu cầu đặt lại mật khẩu";
        var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
            </head>
            <body style='font-family: ""Helvetica Neue"", Helvetica, Arial, sans-serif; background-color: #f4f5f7; margin: 0; padding: 0;'>
                <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f4f5f7; padding: 40px 20px;'>
                    <tr>
                        <td align='center'>
                            <table width='100%' cellpadding='0' cellspacing='0' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.05);'>
                                
                                <tr>
                                    <td style='padding: 30px; text-align: center; background-color: #2563eb;'>
                                        <h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: 600;'>Khôi Phục Mật Khẩu</h1>
                                    </td>
                                </tr>
                                
                                <tr>
                                    <td style='padding: 40px 30px; color: #334155; font-size: 16px; line-height: 1.6;'>
                                        <p style='margin-top: 0;'>Xin chào,</p>
                                        <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Dưới đây là mã xác nhận của bạn:</p>
                                        
                                        <div style='text-align: center; margin: 35px 0;'>
                                            <span style='display: inline-block; padding: 16px 32px; font-size: 28px; font-weight: bold; color: #2563eb; background-color: #eff6ff; border-radius: 8px; letter-spacing: 6px; border: 1px dashed #bfdbfe;'>
                                                {resetCode}
                                            </span>
                                        </div>
                                        
                                        <p>Mã này sẽ hết hạn sau 15 phút. Nếu bạn không yêu cầu thay đổi mật khẩu, bạn có thể an tâm bỏ qua email này. Tài khoản của bạn vẫn an toàn.</p>
                                        <p style='margin-bottom: 0;'>Trân trọng,<br><strong>Đội ngũ hỗ trợ</strong></p>
                                    </td>
                                </tr>
                                
                                <tr>
                                    <td style='padding: 20px; text-align: center; font-size: 13px; color: #94a3b8; background-color: #f8fafc; border-top: 1px solid #e2e8f0;'>
                                        <p style='margin: 0;'>Đây là email tự động, vui lòng không trả lời email này.</p>
                                    </td>
                                </tr>
                                
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

        await SendAsync(to, subject, body, cancellationToken);
    }

    public async Task SendAssessmentReadyEmailAsync(
        string to,
        string userName,
        string assessmentName,
        CancellationToken cancellationToken = default)
    {
        var subject = "Đánh giá đã sẵn sàng";
        var body = $"<h1>Xin chào {userName}</h1><p>Đánh giá '{assessmentName}' của bạn đã sẵn sàng.</p>";
        await SendAsync(to, subject, body, cancellationToken);
    }

    public string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return string.Empty;

        var emailParts = email.Split('@');
        if (emailParts.Length < 2) return email;

        var namePart = emailParts[0];
        var domainPart = emailParts[1];

        var maskedName = namePart.Length > 3
            ? $"{namePart.Substring(0, 2)}***{namePart.Substring(namePart.Length - 1)}"
            : $"{namePart.Substring(0, 1)}***";

        return $"{maskedName}@{domainPart}";
    }
}

public class EmailSendException : Exception
{
    public EmailSendException(string message, Exception? innerException = null) : base(message, innerException) { }
}
