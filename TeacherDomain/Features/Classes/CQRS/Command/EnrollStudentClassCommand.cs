namespace TeacherDomain.Features.Classes.CQRS.Command;
public record EnrollStudentClassCommand(int ClassId) : IRequest<CommitResult>;

