namespace TeacherDomain.Features.Classes.DTO.Command;
public class UnrollStudentFromClassByTeacherRequest
{
    public int ClassId { get; set; }
    public string StudentId { get; set; }
}
