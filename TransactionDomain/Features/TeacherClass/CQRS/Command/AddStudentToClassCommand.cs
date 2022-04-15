using TransactionDomain.Features.TeacherClass.DTO.Command;

namespace TransactionDomain.Features.TeacherClass.CQRS.Command;

public record AddStudentToClassCommand(AddStudentToClassRequest AddStudentToClassRequest) : IRequest<CommitResult>;


