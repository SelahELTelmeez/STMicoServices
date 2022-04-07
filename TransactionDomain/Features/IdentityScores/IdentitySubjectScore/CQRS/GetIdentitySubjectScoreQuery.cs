using TransactionDomain.Features.IdentityScores.IdentitySubjectScore.DTO;

namespace TransactionDomain.Features.IdentityScores.IdentitySubjectScore.CQRS;
public record GetIdentitySubjectScoreQuery(string SubjectId) : IRequest<CommitResult<IdentitySubjectScoreResponse>>;

