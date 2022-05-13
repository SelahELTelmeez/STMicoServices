namespace SharedModule.DTO;

public class StudentQuizResultResponse
{
    public int QuizId { get; set; }
    public int QuizScore { get; set; }
    public int StudentScore { get; set; }
    public Guid StudentId { get; set; }
}
