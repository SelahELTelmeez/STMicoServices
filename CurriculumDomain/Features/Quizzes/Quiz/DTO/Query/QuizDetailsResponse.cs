namespace CurriculumDomain.Features.Quizzes.Quiz.DTO.Query;
public class QuizDetailsResponse
{
    public int Id { get; set; }
    public string? SubjectId { get; set; }
    public int? UnitId { get; set; }
    public int? LessonId { get; set; }
    public List<QuizQuestionResponse> QuestionResponses { get; set; }
    public int Duration { get; set; }
}
