namespace TeacherDomain.Features.Classes.DTO.Query;
public class ClassResponse
{
    public int ClassId { get; set; }
    public string Name { get; set; }
    public string SubjectId { get; set; }
    public string Description { get; set; }
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; }
    public string AvatarUrl { get; set; }
    public bool? IsEnrolled { get; set; }
}
