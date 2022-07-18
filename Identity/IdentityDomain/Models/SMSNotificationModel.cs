namespace IdentityDomain.Models;

public class SMSNotificationModel
{
    public string Mobile { get; set; }
    public string Code { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Msignature { get; set; }
    public string Token { get; set; }
}
