using TransactionDomain.Features.TeacherClass.DTO.Command;

namespace TransactionDomain.Features.TeacherClass.CQRS.Command;

public record InviteStudentToClassCommand(InviteStudentToClassRequest InviteStudentToClassRequest) : IRequest<CommitResult>;

