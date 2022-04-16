using TransactionDomain.Features.TeacherClass.DTO.Command;

namespace TransactionDomain.Features.Classes.CQRS.Command;

public record UpdateClassCommand(UpdateClassRequest UpdateClassRequest) : IRequest<CommitResult>;


