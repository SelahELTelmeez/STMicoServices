using TeacherDomain.Features.TeacherClass.DTO.Command;

namespace TeacherDomain.Features.Classes.CQRS.Command;
public record AcceptStudentEnrollToClassRequestCommand(AcceptStudentEnrollToClassRequest AcceptStudentEnrollToClassRequest) : IRequest<CommitResult>;


