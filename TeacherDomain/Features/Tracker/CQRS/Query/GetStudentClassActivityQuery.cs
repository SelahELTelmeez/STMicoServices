using TeacherDomain.Features.Tracker.DTO.Query;

namespace TeacherDomain.Features.Tracker.CQRS.Query;

public record GetStudentClassActivityQuery(string StudentId, int ClassId) : IRequest<ICommitResults<StudentClassActivityResponse>>;


