namespace TransactionDomain.Features.Classes.CQRS.Command;
public record UnrollStudentClassCommand(int ClassId) : IRequest<CommitResult>;