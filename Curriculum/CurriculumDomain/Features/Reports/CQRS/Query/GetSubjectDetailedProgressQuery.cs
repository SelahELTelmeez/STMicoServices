using SharedModule.DTO;

namespace CurriculumDomain.Features.Reports.CQRS.Query;

public record GetSubjectDetailedProgressQuery(string SubjectId) : IRequest<CommitResult<DetailedProgressResponse>>;


