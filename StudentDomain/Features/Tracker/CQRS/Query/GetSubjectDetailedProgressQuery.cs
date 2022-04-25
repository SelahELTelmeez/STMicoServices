using StudentDomain.Features.Tracker.DTO.Query;

namespace StudentDomain.Features.Tracker.CQRS.Query;
public record GetSubjectDetailedProgressQuery(string SubjectId) : IRequest<CommitResult<DetailedProgressResponse>>;

