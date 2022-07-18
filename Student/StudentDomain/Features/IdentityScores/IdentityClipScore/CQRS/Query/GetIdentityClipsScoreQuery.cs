using StudentDomain.Features.IdentityScores.IdentityClipScore.DTO;

namespace StudentDomain.Features.IdentityScores.IdentityClipScore.CQRS.Query;
public record GetIdentityClipsScoreQuery(int LessonId) : IRequest<ICommitResult<IdentityClipsScoreResponse>>;