using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Quizzes;

public class QuizAnswer : BaseEntity
{
    public FormType Type { get; set; }
    public string Value { get; set; }
    public bool IsCorrectAnswer { get; set; }
    public int QuizId { get; set; }
    [ForeignKey(nameof(QuizId))] public Quiz QuizFK { get; set; }
}
