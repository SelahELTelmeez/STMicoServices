using System.ComponentModel.DataAnnotations.Schema;
using TeacherEntites.Entities.Shared;
using TeacherEntities.Entities.Shared;
using TeacherEntities.Entities.TeacherActivity;

namespace TeacherEntities.Entities.Trackers;

public class TeacherQuizActivityTracker : TrackableEntity
{
    public int ClassId { get; set; }
    public int TeacherQuizId { get; set; }
    public string StudentId { get; set; }
    public ActivityStatus ActivityStatus { get; set; }
    [ForeignKey(nameof(TeacherQuizId))] public TeacherQuiz TeacherQuizFK { get; set; }
}
