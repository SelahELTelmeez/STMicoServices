using TeacherDomain.Features.Quiz.DTO.Query;

namespace TeacherDomain.Features.Assignment.CQRS.Query;
public record class GetQuizzesQuery() : IRequest<ICommitResults<QuizResponse>>;


