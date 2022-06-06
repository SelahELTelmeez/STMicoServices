using TeacherDomain.Features.TeacherClass.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;
public record GetStudentClassesQuery(Guid? StudentId) : IRequest<ICommitResults<StudentClassResponse>>;


