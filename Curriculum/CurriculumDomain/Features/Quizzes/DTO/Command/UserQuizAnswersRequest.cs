namespace CurriculumDomain.Features.Quizzes.DTO.Command;
public class UserQuizAnswersRequest
{
    public int? ClipId { get; set; }
    public int QuizId { get; set; }
    public int TimeSpent { get; set; }
    public List<UserQuizAnswerRequest> QuizAnswerRequests { get; set; }
}

public class UserQuizAnswerRequest
{
    public int QuestionId { get; set; }
    public int AnswerId { get; set; }
}