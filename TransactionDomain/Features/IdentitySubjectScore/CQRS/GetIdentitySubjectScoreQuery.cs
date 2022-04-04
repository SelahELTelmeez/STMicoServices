using MediatR;
using ResultHandler;
using TransactionDomain.Features.IdentitySubjectScore.DTO;

namespace TransactionDomain.Features.IdentitySubjectScore.CQRS;
public record GetIdentitySubjectScoreQuery(string SubjectId) : IRequest<CommitResult<IdentitySubjectScoreResponse>>;

