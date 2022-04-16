using TransactionDomain.Features.Classes.SearchClass.DTO.Query;

namespace TransactionDomain.Features.Classes.SearchClass.CQRS.Query;
public record SearchClassQuery(int ClassId) : IRequest<CommitResult<ClassResponse>>;