namespace TransactionDomain.Features.GetStudentRecentLessonsProgress.DTO;

public class StudentRecentLessonProgressResponse
{
    public string LessonName { get; set; }
    public int LessonPoints { get; set; }
    public float StudentPoints { get; set; }
    public float Progress { get => StudentPoints / (float)LessonPoints; }
}
