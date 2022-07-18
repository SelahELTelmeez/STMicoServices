using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;

public record GetClassesByAssignmentIdQuery(int AssignmentId) : IRequest<ICommitResults<ClassBriefResponse>>;