using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Quizzes;

public class QuizQuestion : BaseEntity
{
    public FormType Type { get; set; }
    public string Value { get; set; }
    public int QuizFormId { get; set; }
    [ForeignKey(nameof(QuizFormId))] public QuizForm QuizFormFK { get; set; }
}
