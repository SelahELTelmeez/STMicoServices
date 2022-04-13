using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Quizzes;
/// <summary>
/// Represents  the student's result for the assigned quiz.
/// </summary>
public class UserQuiz : TrackableEntity
{
    public Guid IdentityUserId { get; set; }
    public int TimeSpentInSec { get; set; }
    public bool IsAnswered { get; set; }
    public int IdentityUserScore { get; set; }
    public int TotalQuizScore { get; set; }
    public int QuizId { get; set; }
    [ForeignKey(nameof(QuizId))] public Quiz QuizFK { get; set; }
}