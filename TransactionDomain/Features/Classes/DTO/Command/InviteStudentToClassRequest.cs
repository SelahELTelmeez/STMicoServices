namespace TransactionDomain.Features.TeacherClass.DTO.Command;

public class InviteStudentToClassRequest
{
    public Guid StudentId { get; set; }
    public int ClassId { get; set; }
    public Guid TeacherId { get; set; }

}
