using TeacherDomain.Features.TeacherClass.DTO.Command;

namespace TeacherDomain.Features.Classes.CQRS.Command;

public record CreateClassCommand(CreateClassRequest CreateClassRequest) : IRequest<ICommitResult<int>>;


