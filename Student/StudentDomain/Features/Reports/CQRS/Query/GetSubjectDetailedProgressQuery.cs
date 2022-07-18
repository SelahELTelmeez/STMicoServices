using SharedModule.DTO;

namespace StudentDomain.Features.Reports.CQRS.Query;
public record GetSubjectDetailedProgressQuery(string SubjectId, string? SudentId) : IRequest<ICommitResult<DetailedProgressResponse>>;

