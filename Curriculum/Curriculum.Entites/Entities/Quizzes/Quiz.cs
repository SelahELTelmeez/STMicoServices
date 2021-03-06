using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Shared;
using CurriculumEntites.Entities.Subjects;
using CurriculumEntites.Entities.Units;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Quizzes;

public class Quiz : TrackableEntity
{
    public string? SubjectId { get; set; }
    public int? LessonId { get; set; }
    public int? UnitId { get; set; }
    public string Creator { get; set; }
    [ForeignKey(nameof(SubjectId))] public Subject? SubjectFK { get; set; }
    [ForeignKey(nameof(LessonId))] public Lesson? LessonFK { get; set; }
    [ForeignKey(nameof(UnitId))] public Unit? UnitFK { get; set; }
    public virtual ICollection<QuizForm>? QuizForms { get; set; }
}
