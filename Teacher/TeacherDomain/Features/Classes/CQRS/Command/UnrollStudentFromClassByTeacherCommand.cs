using TeacherDomain.Features.Classes.DTO.Command;

namespace TeacherDomain.Features.Classes.CQRS.Command;

public record UnrollStudentFromClassByTeacherCommand(UnrollStudentFromClassByTeacherRequest RemoveStudentFromClassRequest) : IRequest<ICommitResult>;


