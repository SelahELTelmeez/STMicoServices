using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;
namespace CurriculumEntites.Entities.Quizzes;
public class QuizForm : TrackableEntity
{
    public QuizQuestion Question { get; set; }
    public QuizAnswer Answer { get; set; }
    public virtual ICollection<QuizAnswer> Answers { get; set; }
    public int DurationInSec { get; set; }
    public string? Hint { get; set; }
    public int? ClipId { get; set; }
    public int QuizId { get; set; }
    [ForeignKey(nameof(ClipId))] public Clip? ClipFK { get; set; }
    [ForeignKey(nameof(QuizId))] public Quiz QuizFK { get; set; }
}