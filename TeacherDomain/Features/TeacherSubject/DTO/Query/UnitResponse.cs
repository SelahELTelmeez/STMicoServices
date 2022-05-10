namespace TeacherDomain.Features.TeacherSubject.DTO.Query;
public class UnitResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string SubjectId { get; set; }
    public List<LessonResponse> Lessons { get; set; }
}
public class LessonResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
