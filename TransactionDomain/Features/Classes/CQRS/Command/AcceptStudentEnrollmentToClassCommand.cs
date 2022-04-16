using TransactionDomain.Features.TeacherClass.DTO.Command;

namespace TransactionDomain.Features.Classes.CQRS.Command;

public record AcceptStudentEnrollmentToClassCommand(AcceptStudentEnrollmentToClassRequest AddStudentToClassRequest) : IRequest<CommitResult>;


