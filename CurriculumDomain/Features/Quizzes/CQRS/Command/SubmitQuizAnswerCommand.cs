using CurriculumDomain.Features.Quizzes.DTO.Command;

namespace CurriculumDomain.Features.Quizzes.CQRS.Command;

public record SubmitQuizAnswerCommand(UserQuizAnswersRequest UserQuizAnswersRequest) : IRequest<CommitResult>;
