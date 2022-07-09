namespace ChatEntities.Entities;

public class ChatSession
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string ConnectionId { get; set; }
}
