using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.StudentLessons;

public class StudentActivityRecord : BaseEntity
{
    public int LessonId { get; set; }
    public Guid StudentId { get; set; }
    public int StudentPoints { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    [ForeignKey(nameof(LessonId))] public Lesson LessonFK { get; set; }
}
