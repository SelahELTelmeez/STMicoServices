using MediatR;
using ResultHandler;
using TransactionDomain.Features.LessonClipScore.DTO;

namespace TransactionDomain.Features.LessonClipScore.CQRS.Query;
public record GetIdentityClipsScoreQuery(int LessonId) : IRequest<CommitResult<IdentityClipsScoreResponse>>;