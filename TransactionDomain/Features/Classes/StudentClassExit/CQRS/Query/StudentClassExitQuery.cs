using TransactionDomain.Features.Classes.StudentClassExit.DTO.Query;

namespace TransactionDomain.Features.Classes.StudentClassExit.CQRS.Query;
public record StudentClassExitQuery(int ClassId) : IRequest<CommitResult<StudentClassResponse>>;