using TeacherDomain.Features.TeacherClass.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;
public record GetStudentClassesQuery(string? StudentId) : IRequest<ICommitResults<StudentClassResponse>>;


