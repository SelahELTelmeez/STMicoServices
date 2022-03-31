namespace TransactionDomain.Features.GetStudentRecentLessonsProgress.DTO;

public class StudentRecentLessonProgressResponse
{
    public string LessonName { get; set; }
    public int LessonPoints { get; set; }
    public float StudentPoints { get; set; }
    public double Progress { get => Math.Round(StudentPoints / (double)LessonPoints, 2); }
}
