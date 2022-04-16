
using TransactionDomain.Features.Classes.DTO.Query;

namespace TransactionDomain.Features.Classes.CQRS.Query;
public record SearchClassQuery(int ClassId) : IRequest<CommitResult<ClassResponse>>;