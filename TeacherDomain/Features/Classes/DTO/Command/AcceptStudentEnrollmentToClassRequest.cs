namespace TeacherDomain.Features.TeacherClass.DTO.Command;
public class AcceptStudentEnrollmentToClassRequest
{
    public int ClassId { get; set; }
    public Guid StudentId { get; set; }
    public int InvitationId { get; set; }
}
