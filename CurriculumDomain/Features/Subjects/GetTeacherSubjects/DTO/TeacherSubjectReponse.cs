using CurriculumDomain.Features.Subjects.GetSubjectUnits.DTO.Query;

namespace CurriculumDomain.Features.Subjects.GetTeacherSubjects.DTO;

public class TeacherSubjectReponse
{
    public string SubjectId { get; set; }
    public string SubjectName { get; set; }
    public string TeacherGuide { get; set; }
    public int Grade { get; set; }
    public int Term { get; set; }
    public string PrimaryIcon { get; set; }
    public string InternalIcon { get; set; }
    public List<UnitResponse> Units { get; set; }
}
