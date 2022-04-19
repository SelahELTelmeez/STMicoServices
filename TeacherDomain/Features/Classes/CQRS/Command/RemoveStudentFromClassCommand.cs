using TeacherDomain.Features.Classes.DTO.Command;

namespace TeacherDomain.Features.Classes.CQRS.Command;

public record RemoveStudentFromClassCommand(RemoveStudentFromClassRequest RemoveStudentFromClassRequest) : IRequest<CommitResult>;


