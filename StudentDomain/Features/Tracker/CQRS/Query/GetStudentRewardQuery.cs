using StudentDomain.Features.Tracker.DTO.Query;

namespace StudentDomain.Features.Tracker.CQRS.Query;

public record GetStudentRewardQuery(int Grade, int TermId) : IRequest<ICommitResult<StudentRewardResponse>>;


