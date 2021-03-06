namespace SharedModule.DTO;


public class SubjectBriefProgressResponse
{
    public string SubjectId { get; set; }
    public string SubjectName { get; set; }
    public double TotalSubjectScore { get; set; }
    public double TotalStudentScore { get; set; }
}
public class DetailedProgressResponse
{
    public string SubjectId { get; set; }
    public string SubjectName { get; set; }
    public int TotalSubjectScore { get; set; }
    public double TotalSubjectStudentScore { get; set; }
    public IEnumerable<DetailedUnitProgress> UnitProgresses { get; set; }
}

public class DetailedUnitProgress
{
    public int UnitId { get; set; }
    public string UnitName { get; set; }
    public IEnumerable<DetailedLessonProgress> LessonProgresses { get; set; }
}

public class DetailedLessonProgress
{
    public int LessonId { get; set; }
    public string LessonName { get; set; }
    public int TotalLessonScore { get; set; }
    public double TotalLessonStudentScore { get; set; }
    public DateTime? ActivityDate { get; set; }
}
