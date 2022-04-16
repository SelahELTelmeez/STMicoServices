using TransactionDomain.Features.TeacherClass.DTO.Command;

namespace TransactionDomain.Features.Classes.CQRS.Command;

public record AddStudentToClassCommand(AddStudentToClassRequest AddStudentToClassRequest) : IRequest<CommitResult>;


