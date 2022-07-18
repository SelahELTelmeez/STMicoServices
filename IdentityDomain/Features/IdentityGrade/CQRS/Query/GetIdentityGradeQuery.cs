using Flaminco.CommitResult;

namespace IdentityDomain.Features.IdentityGrade.CQRS.Query;

public record GetIdentityGradeQuery(string? IdentityId) : IRequest<ICommitResult<int>>;

