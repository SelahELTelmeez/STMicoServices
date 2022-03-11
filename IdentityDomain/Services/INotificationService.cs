using IdentityDomain.Models;
namespace IdentityDomain.Services;
public interface INotificationService
{
    Task<bool> SendEmailAsync(EmailNotificationModel mail, CancellationToken cancellationToken);
    Task<bool> SendSMSAsync(SMSNotificationModel SMSMessage, CancellationToken cancellationToken);
}
