namespace TeacherDomain.Features.Assignment.DTO.Command;
public class CreateAssignmentRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? AttachmentId { get; set; }
    public List<int> Classes { get; set; }
    public string? SubjectName { get; set; }
}
