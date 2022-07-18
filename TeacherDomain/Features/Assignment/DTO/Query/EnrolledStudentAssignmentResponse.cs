using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Assignment.DTO.Query
{
    public class EnrolledStudentAssignmentResponse : EnrolledStudentResponse
    {
        public string? ReplyComment { get; set; }
        public Guid? ReplyAttachmentId { get; set; }
    }
}
