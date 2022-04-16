namespace TransactionDomain.Features.Classes.SearchClassBySubject.DTO.Query;
public class SearchClassBySubjectResponse
{
    public string Name { get; set; }
    public string SubjectId { get; set; }
    public string Description { get; set; }
    public Guid TeacherId { get; set; }
    public bool IsActive { get; set; }
}
