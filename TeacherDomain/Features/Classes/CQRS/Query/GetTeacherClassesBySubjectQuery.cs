
using TeacherDomain.Features.TeacherClass.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;

public record GetTeacherClassesBySubjectQuery(string SubjectId) : IRequest<CommitResults<TeacherClassResponse>>;