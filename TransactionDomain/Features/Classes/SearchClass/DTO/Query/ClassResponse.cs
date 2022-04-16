namespace TransactionDomain.Features.Classes.SearchClass.DTO.Query;
public class ClassResponse
{
    public string Name { get; set; }
    public string SubjectId { get; set; }
    public string Description { get; set; }
    public Guid TeacherId { get; set; }
    public bool IsActive { get; set; }
}
