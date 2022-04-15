namespace TransactionDomain.Models;

public class NotificationModel
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string Token { get; set; }
    public int Type { get; set; }
}
