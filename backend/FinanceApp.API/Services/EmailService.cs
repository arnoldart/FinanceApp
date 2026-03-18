using System.Net;
using System.Net.Mail;

namespace FinanceApp.API.Services;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        var host = _config["Smtp:Host"] ?? throw new InvalidOperationException("Smtp:Host is missing.");
        var portRaw = _config["Smtp:Port"] ?? throw new InvalidOperationException("Smtp:Port is missing.");
        var email = _config["Smtp:Email"] ?? throw new InvalidOperationException("Smtp:Email is missing.");
        var password = _config["Smtp:Password"] ?? throw new InvalidOperationException("Smtp:Password is missing.");

        if (!int.TryParse(portRaw, out var port))
        {
            throw new InvalidOperationException("Smtp:Port is invalid.");
        }

        var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(email, password),
            EnableSsl = true
        };

        var message = new MailMessage(email, to, subject, body)
        {
            IsBodyHtml = isHtml
        };

        await client.SendMailAsync(message);
    }
}
