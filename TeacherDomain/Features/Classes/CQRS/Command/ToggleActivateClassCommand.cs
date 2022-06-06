namespace TeacherDomain.Features.Classes.CQRS.Command;
public record ToggleActivateClassCommand(int ClassId) : IRequest<ICommitResult>;

