using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.StudentLessons;

public class StudentActivityTracker : TrackableEntity
{
    public Guid StudentId { get; set; }
    public int StudentPoints { get; set; }
    public int LearningDurationInSec { get; set; }
    public bool IsActive { get; set; }
    public int ClipId { get; set; }
    [ForeignKey(nameof(ClipId))] public Clip ClipFK { get; set; }
}
