
namespace CurriculumDomain.Features.Quizzes.DTO.Query;
public class QuizQuestionResponse
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string Value { get; set; }
    public string? Hint { get; set; }
    public QuizClipResponse? ClipExplanatory { get; set; }
    public List<QuizAnswerResponse> AnswerResponses { get; set; }
}