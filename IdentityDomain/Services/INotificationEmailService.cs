using IdentityDomain.Models;
using System.Net;
using System.Net.Mail;
using System.Text;
namespace IdentityDomain.Services;
public interface INotificationEmailService
{
    Task SendAsync(EmailNotificationModel mail, CancellationToken cancellationToken);
}
public class NotificationEmailService : INotificationEmailService
{
    public async Task SendAsync(EmailNotificationModel emailModel, CancellationToken cancellationToken)
    {
        ///TODO: Add Email Body.
        MailMessage emailMessage = new MailMessage()
        {
            From = new MailAddress(emailModel.MailFrom, emailModel.DisplayName, Encoding.UTF8),
            Subject = emailModel.MailSubject,
            SubjectEncoding = Encoding.UTF8,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = emailModel.IsBodyHtml,
            Priority = MailPriority.High,
        };
        emailMessage.To.Add(emailModel.MailTo);
        SmtpClient client = new SmtpClient()
        {
            Credentials = new NetworkCredential(emailModel.MailFrom, "Smtp@Stpp#2030"),
            Port = 587,
            Host = "smtp.office365.com",
            EnableSsl = true
        };
        await client.SendMailAsync(emailMessage, cancellationToken);
    }
}