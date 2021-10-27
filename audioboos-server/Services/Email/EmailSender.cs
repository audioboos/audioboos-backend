using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AudioBoos.Server.Services.Email; 

public class EmailSender : IEmailSender {
    public EmailOptions _settings { get; } //set only via Secret Manager

    public EmailSender(IOptions<EmailOptions> optionsAccessor) {
        _settings = optionsAccessor.Value;
    }


    public Task SendEmailAsync(string email, string subject, string message) {
        return Execute(_settings.ServiceKey, subject, message, email);
    }

    public Task Execute(string apiKey, string subject, string message, string email) {
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage {
            From = new EmailAddress(_settings.FromEmail, _settings.FromName),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(email));
        msg.SetClickTracking(true, true);
        return client.SendEmailAsync(msg);
    }
}