using IdentityDomain.Models;

namespace IdentityDomain.Services
{
    public interface INotificationSMSService
    {
        Task<bool> SendAsync(EmailNotificationModel mail);
    }
}
