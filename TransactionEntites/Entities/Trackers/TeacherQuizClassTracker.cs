using System.ComponentModel.DataAnnotations.Schema;
using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.Trackers
{
    public class TeacherQuizClassTracker : TrackableEntity
    {
        public int ClassId { get; set; }
        public int TrackerId { get; set; }
        [ForeignKey(nameof(TrackerId))] public TeacherQuizTracker TeacherQuizTrackerFK { get; set; }
    }
}
