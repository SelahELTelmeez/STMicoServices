using ResultHandler;

namespace IdentityDomain.Features.IdentityGrade.CQRS.Query;

public record GetIdentityGradeQuery() : IRequest<CommitResult<int>>;

