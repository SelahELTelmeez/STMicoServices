using TransactionDomain.Features.TeacherClass.DTO.Command;

namespace TransactionDomain.Features.TeacherClass.CQRS.Command;

public record CreateClassCommand(CreateClassRequest CreateClassRequest) : IRequest<CommitResult>;


