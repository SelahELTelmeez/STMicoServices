namespace TeacherDomain.Features.Assignment.DTO.Query;
public class AssignmentResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime EndDate { get; set; }
    public string? LessonName { get; set; }
    public string? SubjectName { get; set; }
    public string? ClassName { get; set; }
    public int EnrolledCounter { get; set; }
}