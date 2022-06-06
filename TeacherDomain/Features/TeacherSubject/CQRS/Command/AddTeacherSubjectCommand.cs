namespace TeacherDomain.Features.TeacherSubject.CQRS.Command;
public record AddTeacherSubjectCommand(IEnumerable<string> SubjectIds) : IRequest<ICommitResult>;

