using IdentityDomain.Models;
namespace Auth.Services.NotificationService;
public interface INotificationEmailService
{
    Task<bool> SendAsync(EmailNotificationModel mail);
}
