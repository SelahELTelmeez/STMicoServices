using CurriculumDomain.Features.Quiz.DTO.Command;
using ResultHandler;

namespace CurriculumDomain.Features.Quiz.CQRS.Command;
public record CreateQuizCommand(QuizRequest QuizRequest) : IRequest<CommitResult<int>>;