namespace ChatEntities.Entities;

public class Message
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Content { get; set; }
    public string FromId { get; set; }
    public string ToId { get; set; }
}
