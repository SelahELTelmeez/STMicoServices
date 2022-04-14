using CurriculumDomain.Features.Quizzes.Quiz.DTO.Command;

namespace CurriculumDomain.Features.Quizzes.Quiz.CQRS.Command;

public record SubmitQuizAnswerCommand(UserQuizAnswersRequest UserQuizAnswersRequest) : IRequest<CommitResult>;
