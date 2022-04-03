using MediatR;
using ResultHandler;

namespace TransactionDomain.Features.IdentitySubjectScore.CQRS;

public record GetIdentitySubjectScoreQuery(string SubjectId) : IRequest<CommitResult<float>>;

