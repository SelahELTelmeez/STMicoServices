namespace TeacherDomain.Features.Classes.DTO.Query;

public class EnrolleeDetailsResponse
{
    public string EnrolleeName { get; set; }
    public IEnumerable<SubjectBriefResponse> Subjects { get; set; }
    public IEnumerable<ClassBriefResponse> Classes { get; set; }
    public int ClassCounter { get; set; }
}
