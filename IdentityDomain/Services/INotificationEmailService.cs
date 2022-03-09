using IdentityDomain.Models;

namespace IdentityDomain.Services;
public interface INotificationEmailService
{
    Task<bool> SendAsync(EmailNotificationModel mail);
}
