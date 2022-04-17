namespace TransactionDomain.Features.Assignment.DTO;

public class ReplyAssignmentRequest
{
    public int AssignmentActivityTrackerId { get; set; }
    public string ReplyComment { get; set; }
    public string ReplyAttachmentUrl { get; set; }
}
