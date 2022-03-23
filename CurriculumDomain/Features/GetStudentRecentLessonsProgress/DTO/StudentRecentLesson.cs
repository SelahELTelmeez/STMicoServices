namespace CurriculumDomain.Features.GetStudentRecentLessonsProgress.DTO;

public class StudentRecentLessonProgress
{
    public string LessonName { get; set; }
    public int LessonPoints { get; set; }
    public int StudentPoints { get; set; }
    public float Progress { get => StudentPoints / (float)LessonPoints; }
}
