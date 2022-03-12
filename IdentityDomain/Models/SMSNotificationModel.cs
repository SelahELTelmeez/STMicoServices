namespace IdentityDomain.Models;

public class SMSNotificationModel
{
    public string MobileNumber { get; set; }
    public string OTPCode { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Msignature { get; set; }
    public string Token { get; set; }
}
