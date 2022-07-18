namespace TeacherDomain.Features.TeacherClass.DTO.Command;
public class AcceptStudentEnrollToClassRequest
{
    public int ClassId { get; set; }
    public string StudentId { get; set; }
    public int InvitationId { get; set; }
}
