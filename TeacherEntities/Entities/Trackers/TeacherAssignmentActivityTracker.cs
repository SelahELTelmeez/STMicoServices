using System.ComponentModel.DataAnnotations.Schema;
using TeacherEntites.Entities.Shared;
using TeacherEntities.Entities.Shared;
using TeacherEntities.Entities.TeacherActivity;

namespace TeacherEntities.Entities.Trackers;

public class TeacherAssignmentActivityTracker : BaseEntity
{
    public int ClassId { get; set; }
    public int TeacherAssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public ActivityStatus ActivityStatus { get; set; }
    public string ReplyComment { get; set; }
    public string ReplyAttachmentUrl { get; set; }
    [ForeignKey(nameof(TeacherAssignmentId))] public TeacherAssignment TeacherAssignmentFK { get; set; }
}
