namespace TransactionDomain.Features.Tracker.DTO;

public class StudentRecentLessonProgressResponse
{
    public string LessonName { get; set; }
    public int LessonPoints { get; set; }
    public int StudentPoints { get; set; }
    public double Progress { get => Math.Round(StudentPoints / (double)LessonPoints, 2); }
}
