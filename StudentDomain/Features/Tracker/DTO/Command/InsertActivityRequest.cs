namespace StudentDomain.Features.Activities.DTO.Command;
public class InsertActivityRequest
{
    public int ClipId { get; set; }
    public int LessonId { get; set; }
    public string SubjectId { get; set; }
}
