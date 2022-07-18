namespace IdentityDomain.Models;
public class EmailNotificationModel
{
    public string MailTo { get; set; }
    public string? MailToName { get; set; }
    public string MailFrom { get; set; }
    public string? DisplayName { get; set; }
    public string? MailSubject { get; set; }
    public string MailBody { get; set; }
    public bool IsBodyHtml { get; set; }
}