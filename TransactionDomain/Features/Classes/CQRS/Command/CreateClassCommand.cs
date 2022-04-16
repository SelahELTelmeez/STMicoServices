using TransactionDomain.Features.TeacherClass.DTO.Command;

namespace TransactionDomain.Features.Classes.CQRS.Command;

public record CreateClassCommand(CreateClassRequest CreateClassRequest) : IRequest<CommitResult<int>>;


