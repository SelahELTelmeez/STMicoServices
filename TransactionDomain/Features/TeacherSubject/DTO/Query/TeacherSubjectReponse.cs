namespace TransactionDomain.Features.TeacherSubject.DTO.Query;

public class TeacherSubjectReponse
{
    public string SubjectId { get; set; }
    public string SubjectName { get; set; }
    public string TeacherGuide { get; set; }
    public string GradeName { get; set; }
    public int GradeValue { get; set; }
    public int Term { get; set; }
    public string Icon { get; set; }
}
