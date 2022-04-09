using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.Trackers;

public class StudentActivityTracker : TrackableEntity
{
    public Guid StudentId { get; set; }
    public double StudentPoints { get; set; }
    public int LearningDurationInSec { get; set; }
    public bool IsActive { get; set; }
    public int Code { get; set; }
    public int Progress { get; set; }
    public int ClipId { get; set; }
    public int LessonId { get; set; }
    public string SubjectId { get; set; }
}
