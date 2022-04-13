namespace CurriculumDomain.Features.Quizzes.Quiz.DTO.Query;
public class QuizAnswerResponse
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Value { get; set; }
    public bool IsCorrect { get; set; }
}
