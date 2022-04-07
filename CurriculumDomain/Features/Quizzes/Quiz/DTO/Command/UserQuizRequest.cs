namespace CurriculumDomain.Features.Quizzes.Quiz.DTO.Command;
public class UserQuizRequest
{
    public Guid IdentityUserId { get; set; }
    public int TimeSpentInSec { get; set; }
    public int DurationInSec { get; set; }
    public bool IsAnswered { get; set; }
    public int IdentityUserScore { get; set; }
    public int TotalQuizScore { get; set; }
    public int QuizId { get; set; }
}