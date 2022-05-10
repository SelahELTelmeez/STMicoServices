using SharedModule.DTO;
using StudentDomain.Features.Tracker.DTO.Query;

namespace StudentDomain.Features.Tracker.CQRS.Query;
public record GetStudentQuizzesResultQuery(StudentQuizResultRequest StudentQuizResultRequest) : IRequest<CommitResults<StudentQuizResultResponse>>;


