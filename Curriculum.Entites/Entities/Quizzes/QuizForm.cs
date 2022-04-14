using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;
namespace CurriculumEntites.Entities.Quizzes;

/// <summary>
/// Represents the quiz question and answer
/// </summary>
public class QuizForm : TrackableEntity
{
    public int DurationInSec { get; set; }
    public string? Hint { get; set; }
    public int? ClipId { get; set; }
    public int? QuestionId { get; set; }
    public int QuizId { get; set; }
    [ForeignKey(nameof(QuestionId))] public QuizQuestion? Question { get; set; }
    [ForeignKey(nameof(ClipId))] public Clip? ClipFK { get; set; }
    [ForeignKey(nameof(QuizId))] public Quiz QuizFK { get; set; }
    public virtual ICollection<QuizAnswer> Answers { get; set; }
}