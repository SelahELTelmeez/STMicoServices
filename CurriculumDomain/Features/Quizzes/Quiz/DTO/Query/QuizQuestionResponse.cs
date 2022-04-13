using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;

namespace CurriculumDomain.Features.Quizzes.Quiz.DTO.Query;
public class QuizQuestionResponse
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Value { get; set; }
    public string? Hint { get; set; }
    public QuizClipResponse? ClipExplanatory { get; set; }
    public List<QuizAnswerResponse> AnswerResponses { get; set; }
}