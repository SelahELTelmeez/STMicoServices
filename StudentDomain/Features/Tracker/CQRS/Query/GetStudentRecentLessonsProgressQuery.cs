using StudentDomain.Features.Tracker.DTO;

namespace StudentDomain.Features.Tracker.CQRS.Query;
public record GetStudentRecentLessonsProgressQuery : IRequest<CommitResults<StudentRecentLessonProgressResponse>>;