using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Quizzes;

/// <summary>
/// Represents a user attempt to solve the quiz form, it acts as a log for the student's answers
/// </summary>
public class QuizAttempt : TrackableEntity
{
    public Guid StudentUserId { get; set; }
    public int? QuizFormId { get; set; }
    public int? UserAnswerId { get; set; }
    public bool IsCorrect { get; set; }
    [ForeignKey(nameof(QuizFormId))] public QuizForm? QuizForm { get; set; }
    [ForeignKey(nameof(UserAnswerId))] public QuizAnswer? UserAnswer { get; set; }
}
