namespace TeacherDomain.Features.Assignment.DTO.Command;

public class ReplyAssignmentRequest
{
    public int AssignmentActivityTrackerId { get; set; }
    public string ReplyComment { get; set; }
    public Guid ReplyAttachmentId { get; set; }
}
