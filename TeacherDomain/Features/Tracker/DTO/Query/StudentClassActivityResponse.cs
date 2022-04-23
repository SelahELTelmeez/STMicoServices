namespace TeacherDomain.Features.Tracker.DTO.Query;

public class StudentClassActivityResponse
{
    public int ActivityType { get; set; }

    public string Title { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? TotalScore { get; set; }

    public int? StudentScore { get; set; }
}
