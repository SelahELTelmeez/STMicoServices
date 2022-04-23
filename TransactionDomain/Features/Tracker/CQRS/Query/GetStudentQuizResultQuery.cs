using TransactionDomain.Features.Tracker.DTO.Query;

namespace TransactionDomain.Features.Tracker.CQRS.Query;

public record GetStudentQuizzesResultQuery(StudentQuizResultRequest StudentQuizResultRequest) : IRequest<CommitResults<StudentQuizResultResponse>>;


