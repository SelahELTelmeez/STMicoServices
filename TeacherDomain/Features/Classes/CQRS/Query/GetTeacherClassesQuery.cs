using TeacherDomain.Features.TeacherClass.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;

public record GetTeacherClassesQuery() : IRequest<ICommitResults<TeacherClassResponse>>;


