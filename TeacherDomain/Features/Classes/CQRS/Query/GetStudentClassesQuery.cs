using TeacherDomain.Features.TeacherClass.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;
public record GetStudentClassesQuery() : IRequest<CommitResults<StudentClassResponse>>;


