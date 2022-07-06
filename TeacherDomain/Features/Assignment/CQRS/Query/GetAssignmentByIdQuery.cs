using TeacherDomain.Features.Assignment.DTO.Query;

namespace TeacherDomain.Features.Assignment.CQRS.Query;

public record GetAssignmentByIdQuery(int Id, int ClassId) : IRequest<ICommitResult<AssignmentResponse>>;


