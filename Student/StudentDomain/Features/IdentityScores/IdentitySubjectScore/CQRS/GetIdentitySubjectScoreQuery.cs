using StudentDomain.Features.IdentityScores.IdentitySubjectScore.DTO;

namespace StudentDomain.Features.IdentityScores.IdentitySubjectScore.CQRS;
public record GetIdentitySubjectScoreQuery(string SubjectId) : IRequest<ICommitResult<IdentitySubjectScoreResponse>>;

