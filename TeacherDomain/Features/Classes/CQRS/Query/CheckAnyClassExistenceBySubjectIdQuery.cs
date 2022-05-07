namespace TeacherDomain.Features.Classes.CQRS.Query;

public record CheckAnyClassExistenceBySubjectIdQuery(string SubjectId) : IRequest<CommitResult<bool>>;

