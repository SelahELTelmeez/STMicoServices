using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Quizzes;

public class QuizAnswer : BaseEntity
{
    public QuizFormType AnswerType { get; set; }
    public string Value { get; set; }
    public int QuizId { get; set; }
    [ForeignKey(nameof(QuizId))] public Quiz QuizFK { get; set; }
}
