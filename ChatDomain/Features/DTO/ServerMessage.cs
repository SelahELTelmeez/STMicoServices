namespace ChatDomain.Features.DTO;

public class ServerMessage
{
    public Guid FromId { get; set; }
    public Guid ToId { get; set; }
    public string Content { get; set; }
}
