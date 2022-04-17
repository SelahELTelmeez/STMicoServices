using System.ComponentModel.DataAnnotations.Schema;
using TransactionEntites.Entities.Shared;
using TransactionEntites.Entities.TeacherActivity;

namespace TransactionEntites.Entities.Trackers;

public class TeacherQuizActivityTracker : BaseEntity
{
    public int ClassId { get; set; }
    public int TeacherQuizId { get; set; }
    public Guid StudentId { get; set; }
    public ActivityStatus ActivityStatus { get; set; }
    [ForeignKey(nameof(TeacherQuizId))] public TeacherQuiz TeacherQuizFK { get; set; }
}
