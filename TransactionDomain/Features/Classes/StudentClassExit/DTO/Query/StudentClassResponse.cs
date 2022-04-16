namespace TransactionDomain.Features.Classes.StudentClassExit.DTO.Query;
public class StudentClassResponse
{
    public int ClassId { get; set; }
    public Guid StudentId { get; set; }
    public bool IsActive { get; set; }
}