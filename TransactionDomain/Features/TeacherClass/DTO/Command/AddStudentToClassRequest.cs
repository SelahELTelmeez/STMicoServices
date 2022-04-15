namespace TransactionDomain.Features.TeacherClass.DTO.Command;

public class AddStudentToClassRequest
{
    public int ClassId { get; set; }
    public int InvitationId { get; set; }
}
