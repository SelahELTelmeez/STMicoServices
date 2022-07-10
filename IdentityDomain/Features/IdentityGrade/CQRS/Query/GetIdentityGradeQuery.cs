using Flaminco.CommitResult;

namespace IdentityDomain.Features.IdentityGrade.CQRS.Query;

public record GetIdentityGradeQuery(Guid? IdentityId) : IRequest<ICommitResult<int>>;

