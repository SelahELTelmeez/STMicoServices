namespace TransactionDomain.Features.Classes.SearchClassByTeacher.DTO.Query;
public class SearchClassByTeacherResponse
{
    public string Name { get; set; }
    public string SubjectId { get; set; }
    public string Description { get; set; }
    public Guid TeacherId { get; set; }
    public bool IsActive { get; set; }
}
