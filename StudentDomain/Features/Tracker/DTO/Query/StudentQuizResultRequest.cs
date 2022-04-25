namespace StudentDomain.Features.Tracker.DTO.Query;
public class StudentQuizResultRequest
{
    public IEnumerable<int> QuizIds { get; set; }

    public Guid StudentId { get; set; }
}
