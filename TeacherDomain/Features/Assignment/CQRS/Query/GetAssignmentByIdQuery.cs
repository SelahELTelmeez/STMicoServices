using TeacherDomain.Features.Assignment.DTO.Query;

namespace TeacherDomain.Features.Assignment.CQRS.Query;

public record GetAssignmentByIdQuery(int Id) : IRequest<ICommitResult<AssignmentResponse>>;


