namespace TeacherDomain.Features.TeacherClass.DTO.Query;
public class TeacherClassResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SubjectId { get; set; }
    public string Description { get; set; }
    public int EnrollersCount { get; set; }
    public bool IsActive { get; set; }
}