namespace StudentDomain.Features.Tracker.DTO.Query;

public class ProgressCalenderResponse
{
    public IEnumerable<DateTime> ActivityDates { get; set; }
    public DateTime StartDate { get; set; }
    public int MaxLearningDuration { get; set; }
    public DateTime MaxLearningDurationDate { get; set; }
}
