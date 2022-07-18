namespace ChatDomain.Features.DTO;

public class ServerMessage
{
    public string FromId { get; set; }
    public string ToId { get; set; }
    public string Content { get; set; }
}
