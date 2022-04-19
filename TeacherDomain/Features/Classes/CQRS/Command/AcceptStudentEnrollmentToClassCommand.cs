using TeacherDomain.Features.TeacherClass.DTO.Command;

namespace TeacherDomain.Features.Classes.CQRS.Command;
public record AcceptStudentEnrollmentToClassCommand(AcceptStudentEnrollmentToClassRequest AddStudentToClassRequest) : IRequest<CommitResult>;


