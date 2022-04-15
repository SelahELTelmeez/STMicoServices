namespace TransactionDomain.Features.TeacherClass.DTO.Command;

public class UpdateClassRequest
{
    public int ClassId { get; set; }
    public string Name { get; set; }
    public string SubjectId { get; set; }
    public string Description { get; set; }
}
