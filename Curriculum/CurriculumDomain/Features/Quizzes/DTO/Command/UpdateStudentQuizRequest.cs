namespace CurriculumDomain.Features.Quizzes.DTO.Command;

public class UpdateStudentQuizRequest
{
    public int TimeSpentInSec { get; set; }
    public int StudentUserScore { get; set; }
    public int TotalQuizScore { get; set; }
    public bool IsAnswered { get; set; }
    public int QuizId { get; set; }
    public int? ClipId { get; set; }
}
