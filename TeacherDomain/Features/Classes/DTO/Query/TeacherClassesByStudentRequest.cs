namespace TeacherDomain.Features.Classes.DTO.Query;

public class TeacherClassesByStudentRequest
{
    public Guid StudenId { get; set; }
    public Guid TeacherId { get; set; }
}
