namespace CurriculumDomain.Features.Quiz.DTO.Command;
public class QuizRequest
{
    public int? SubjectId { get; set; }
    public int? LessonId { get; set; }
    public int? UnitId { get; set; }
    public Guid Creator { get; set; }
    public List<QuizFormRequest> QuizFormRequests { get; set; }
    public List<UserQuizRequest> UserQuizRequests { get; set; }
}
