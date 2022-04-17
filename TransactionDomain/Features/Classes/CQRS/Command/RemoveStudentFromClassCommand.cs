using TransactionDomain.Features.Classes.DTO.Command;

namespace TransactionDomain.Features.Classes.CQRS.Command;

public record RemoveStudentFromClassCommand(RemoveStudentFromClassRequest RemoveStudentFromClassRequest) : IRequest<CommitResult>;


