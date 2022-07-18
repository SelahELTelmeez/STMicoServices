namespace TeacherDomain.Features.Classes.CQRS.Command;
public record RequestEnrollToClassCommand(int ClassId) : IRequest<ICommitResult>;
