using TransactionDomain.Features.TeacherClass.DTO.Command;

namespace TransactionDomain.Features.TeacherClass.CQRS.Command;

public record UpdateClassCommand(UpdateClassRequest UpdateClassRequest) : IRequest<CommitResult>;


