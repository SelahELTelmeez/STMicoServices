using CurriculumEntites.Entities.Curriculums;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Shared;
using CurriculumEntites.Entities.Units;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Quizzes;

public class Quiz : TrackableEntity
{
    public int? CurriculumId { get; set; }
    public int? LessonId { get; set; }
    public int? UnitId { get; set; }
    public Guid Creator { get; set; }
    [ForeignKey(nameof(CurriculumId))] public Curriculum? CurriculumFK { get; set; }
    [ForeignKey(nameof(LessonId))] public Lesson? LessonFK { get; set; }
    [ForeignKey(nameof(UnitId))] public Unit? UnitFK { get; set; }
    public virtual ICollection<QuizForm>? QuizForms { get; set; }
    public virtual ICollection<UserQuiz>? UserQuizs { get; set; }
}
