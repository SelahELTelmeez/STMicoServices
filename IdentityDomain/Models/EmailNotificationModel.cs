namespace IdentityDomain.Models;
public class EmailNotificationModel
{
    public string MailTo { get; set; }
    public string MailFrom { get; set; }
    public string? MailCc { get; set; }
    public string? MailBcc { get; set; }
    public string? MailSubject { get; set; }
    public string MailBody { get; set; }
    public bool IsBodyHtml { get; set; }
}