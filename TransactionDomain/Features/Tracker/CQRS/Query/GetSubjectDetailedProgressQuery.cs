using TransactionDomain.Features.Tracker.DTO.Query;

namespace TransactionDomain.Features.Tracker.CQRS.Query;

public record GetSubjectDetailedProgressQuery(string SubjectId) : IRequest<CommitResult<DetailedProgressResponse>>;

