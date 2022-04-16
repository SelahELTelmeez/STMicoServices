namespace TransactionDomain.Features.TeacherClass.DTO.Command;
public class CreateClassRequest
{
    public string Name { get; set; }
    public string SubjectId { get; set; }
    public string Description { get; set; }

}
