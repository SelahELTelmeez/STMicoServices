namespace CurriculumDomain.Features.Subjects.CQRS.Query;

public record CheckAnyMCQExistenceBySubjectIdQuery(string SubjectId) : IRequest<CommitResult<bool>>;