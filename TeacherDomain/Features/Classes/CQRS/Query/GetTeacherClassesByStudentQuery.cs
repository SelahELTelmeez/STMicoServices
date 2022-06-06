
using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;

public record GetTeacherClassesByStudentQuery (TeacherClassesByStudentRequest Request) : IRequest<ICommitResult<TeacherClassesByStudentResponse>>;
