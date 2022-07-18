using TeacherDomain.Features.TeacherClass.DTO.Command;

namespace TeacherDomain.Features.Classes.CQRS.Command;
public record UpdateClassCommand(UpdateClassRequest UpdateClassRequest) : IRequest<ICommitResult>;


