namespace StudentDomain.Features.Tracker.DTO.Query;

public class StudentRewardResponse
{
    public double StudentPoints { get; set; }
    public double TotalPoints { get; set; }
    public double OtherRewardPointsValue { get; set; }
    public double OtherRewardPointsMaxValue { get; set; }
    public IEnumerable<Subject> Subjects { get; set; }

}
public class Subject
{
    public string SubjectName { get; set; }
    public IEnumerable<PrizesList> PrizesList { get; set; }
}
public class PrizesList
{
    public int Sort { get; set; }
    public bool IsActive { get; set; }
    public string Source { get; set; }
}
