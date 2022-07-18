namespace StudentDomain.Features.Tracker.CQRS.Query;

public record GetQuizIdForClipQuery(int ClipId) : IRequest<ICommitResult<int?>>;

