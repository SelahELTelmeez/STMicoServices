using TeacherDomain.Features.Tracker.DTO.Query;

namespace TeacherDomain.Features.Tracker.CQRS.Query;

public record GetStudentClassActivityQuery(Guid StudentId) : IRequest<CommitResults<StudentClassActivityResponse>>;


