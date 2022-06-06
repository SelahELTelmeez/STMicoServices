using SharedModule.DTO;

namespace StudentDomain.Features.Reports.CQRS.Query;
public record GetSubjectDetailedProgressQuery(string SubjectId, Guid? SudentId) : IRequest<ICommitResult<DetailedProgressResponse>>;

