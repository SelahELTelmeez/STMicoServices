using TransactionDomain.Features.Classes.DTO.Query;

namespace TransactionDomain.Features.Classes.CQRS.Query;

public record GetActivitiesByClassQuery(int ClassId) : IRequest<CommitResults<ClassActivityResponse>>;
