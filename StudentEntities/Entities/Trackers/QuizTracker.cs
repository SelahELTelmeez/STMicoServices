using StudentEntities.Entities.Shared;

namespace StudentEntities.Entities.Trackers;
/// <summary>
/// Represents  the student's result for the assigned quiz.
/// </summary>
public class QuizTracker : TrackableEntity
{
    public Guid StudentUserId { get; set; }
    public int TimeSpentInSec { get; set; }
    public int StudentUserScore { get; set; }
    public int TotalQuizScore { get; set; }
    public bool IsAnswered { get; set; }
    public int QuizId { get; set; }
}
