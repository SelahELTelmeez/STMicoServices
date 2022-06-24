namespace TeacherDomain.Features.Quiz.DTO.Command;
public class ReplyQuizRequest
{
    public int TeacherQuizId { get; set; }
    public UserQuizAnswersRequest StudentAnswers { get; set; }
}

public class UserQuizAnswersRequest
{
    public int QuizId { get; set; }
    public int TimeSpent { get; set; }
    public List<UserQuizAnswerRequest> QuizAnswerRequests { get; set; }
}

public class UserQuizAnswerRequest
{
    public int QuestionId { get; set; }
    public int AnswerId { get; set; }
}