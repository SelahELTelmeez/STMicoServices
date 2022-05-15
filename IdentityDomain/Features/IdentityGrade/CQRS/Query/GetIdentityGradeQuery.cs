using ResultHandler;

namespace IdentityDomain.Features.IdentityGrade.CQRS.Query;

public record GetIdentityGradeQuery(Guid? IdentityId) : IRequest<CommitResult<int>>;

