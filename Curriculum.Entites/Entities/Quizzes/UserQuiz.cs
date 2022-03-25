using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Quizzes;
public class UserQuiz : TrackableEntity
{
    public Guid IdentityUserId { get; set; }
    public int TimeSpentInSec { get; set; }
    public int DurationInSec { get; set; }
    public bool IsAnswered { get; set; }
    public int IdentityScore { get; set; }
    public int QuizId { get; set; }
    [ForeignKey(nameof(QuizId))] public Quiz QuizFK { get; set; }
}