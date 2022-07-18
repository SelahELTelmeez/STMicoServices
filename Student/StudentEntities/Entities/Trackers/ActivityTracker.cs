using StudentEntities.Entities.Shared;

namespace StudentEntities.Entities.Trackers;
public class ActivityTracker : TrackableEntity
{
    public string StudentId { get; set; }
    public double StudentPoints { get; set; }
    public int LearningDurationInSec { get; set; }
    public bool IsActive { get; set; }
    public int Code { get; set; }
    public int Progress { get; set; }
    public int ClipId { get; set; }
    public int LessonId { get; set; }
    public string SubjectId { get; set; }
    public int? GradeId { get; set; }
    public string? LearningObjectAsJson { get; set; }
}
