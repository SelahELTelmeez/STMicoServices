namespace TransactionDomain.Features.TeacherClass.CQRS.Command;

public record ToggleActivateClassCommand(int ClassId) : IRequest<CommitResult>;

