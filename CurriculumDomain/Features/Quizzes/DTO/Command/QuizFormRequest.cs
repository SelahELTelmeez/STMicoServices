namespace CurriculumDomain.Features.Quizzes.DTO.Command;
public class QuizFormRequest
{
    public int DurationInSec { get; set; }
    public string? Hint { get; set; }
    public int? ClipId { get; set; }
    public int QuestionId { get; set; }
    public int QuizId { get; set; }
}
