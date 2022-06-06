namespace TeacherDomain.Features.Classes.CQRS.Command;
public record UnrollFromClassCommand(int ClassId) : IRequest<ICommitResult>;