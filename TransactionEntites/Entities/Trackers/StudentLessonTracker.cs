using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.Trackers;

public class StudentLessonTracker : TrackableEntity
{
    public Guid StudentId { get; set; }
    public float StudentPoints { get; set; }
    public int LessonId { get; set; }
    public DateTime LastDateTime { get; set; }
}
