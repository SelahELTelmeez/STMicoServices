namespace StudentDomain.Features.Tracker.DTO;
public class StudentRecentLessonProgressResponse
{
    public string LessonName { get; set; }
    public int LessonPoints { get; set; }
    public double StudentPoints { get; set; }
    public double Progress { get => Math.Round(StudentPoints / LessonPoints, 2); }
}
