using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Classes.CQRS.Query;

public record GetClassesByQuizIdQuery(int QuizId) : IRequest<ICommitResults<ClassBriefResponse>>;


