using MediatR;
using ResultHandler;
using TransactionDomain.Features.LessonClipScore.DTO;

namespace TransactionDomain.Features.LessonClipScore.CQRS;
public record LessonClipScoreQuery(int LessonId) : IRequest<CommitResult<LessonClipScoreResponse>>;