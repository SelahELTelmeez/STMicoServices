namespace TransactionDomain.Features.Classes.DTO.Command;

public class RemoveStudentFromClassRequest
{
    public int ClassId { get; set; }
    public Guid StudentId { get; set; }
}
