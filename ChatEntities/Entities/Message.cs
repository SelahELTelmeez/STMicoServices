namespace ChatEntities.Entities;

public class Message
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Content { get; set; }
    public Guid FromId { get; set; }
    public Guid ToId { get; set; }
}
