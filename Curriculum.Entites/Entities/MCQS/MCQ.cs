using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.MCQS;

public class MCQ : TrackableEntity
{
    public string Code { get; set; }
    public MCQQuestion Question { get; set; }
    public int DurationInSec { get; set; }
    public string? Hint { get; set; }
    public int LessonId { get; set; }
    public int? ClipId { get; set; }
    [ForeignKey(nameof(ClipId))] public Clip? ClipFK { get; set; }
    [ForeignKey(nameof(LessonId))] public Lesson LessonFK { get; set; }
    public virtual ICollection<MCQAnswer> Answers { get; set; }
}
