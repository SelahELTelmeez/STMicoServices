namespace TransactionDomain.Features.Classes.CQRS.Command;

public record ToggleActivateClassCommand(int ClassId) : IRequest<CommitResult>;

