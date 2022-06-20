namespace TeacherDomain.Features.Tracker.DTO.Query;

public class DetailedClassActivity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int ActivityType { get; set; }
    public bool IsFinished { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? QuizScore { get; set; }
    public int? StudentScore { get; set; }
}
