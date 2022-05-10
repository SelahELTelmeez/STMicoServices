namespace SharedModule.DTO;

public class TeacherSubjectResponse
{
    public string SubjectId { get; set; }
    public string SubjectName { get; set; }
    public string TeacherGuide { get; set; }
    public int Grade { get; set; }
    public string GradeName { get; set; }
    public string GradeShortName { get; set; }
    public int Term { get; set; }
    public string PrimaryIcon { get; set; }
    public string InternalIcon { get; set; }
    public IEnumerable<UnitResponse> Units { get; set; }
}
public class UnitResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string SubjectId { get; set; }
    public IEnumerable<LessonResponse> Lessons { get; set; }
}
public class LessonResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

