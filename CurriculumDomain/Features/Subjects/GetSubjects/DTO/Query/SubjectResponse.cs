namespace CurriculumDomain.Features.Subjects.GetSubjects.DTO.Query;
public class SubjectResponse
{
    public string Id { get; set; }
    public string? Title { get; set; }
    public int Grade { get; set; }
    public int Term { get; set; }
    public bool? IsAppShow { get; set; }
    public int? RewardPoints { get; set; }
    public string? TeacherGuide { get; set; }
    public string? FullyQualifiedName { get; set; }
    public string? ShortName { get; set; }
    public string PrimaryIcon { get; set; }
    public string InternalIcon { get; set; }

}
