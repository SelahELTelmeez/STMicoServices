using SharedModule.DTO;

namespace StudentDomain.Features.Tracker.CQRS.Query;

public record GetStudentsQuizzesResultQuery(StudentsQuizResultRequest StudentsQuizResultRequest) : IRequest<CommitResults<StudentQuizResultResponse>>;


