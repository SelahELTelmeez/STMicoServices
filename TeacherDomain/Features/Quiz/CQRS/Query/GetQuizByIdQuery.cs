using TeacherDomain.Features.Quiz.DTO.Query;

namespace TeacherDomain.Features.Quiz.CQRS.Query;

public record GetQuizByIdQuery(int Id, int ClassId) : IRequest<ICommitResult<QuizResponse>>;


