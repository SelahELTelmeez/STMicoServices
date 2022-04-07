using MediatR;
using ResultHandler;
using TransactionDomain.Features.IdentityScores.IdentityClipScore.DTO;

namespace TransactionDomain.Features.IdentityScores.IdentityClipScore.CQRS.Query;
public record GetIdentityClipsScoreQuery(int LessonId) : IRequest<CommitResult<IdentityClipsScoreResponse>>;