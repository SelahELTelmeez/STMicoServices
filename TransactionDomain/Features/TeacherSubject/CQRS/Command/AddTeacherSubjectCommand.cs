namespace TransactionDomain.Features.TeacherSubject.CQRS.Command;

public record AddTeacherSubjectCommand(IEnumerable<string> SubjectIds) : IRequest<CommitResult>;

