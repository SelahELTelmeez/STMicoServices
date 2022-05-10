using SharedModule.DTO;

namespace CurriculumDomain.Features.Subjects.GetDetailedProgress.CQRS.Query;

public record GetSubjectDetailedProgressQuery(string SubjectId) : IRequest<CommitResult<DetailedProgressResponse>>;


