namespace TransactionDomain.Features.Tracker.DTO.Query;
public class ClipActivityResponse
{
    public int StudentScore { get; set; }
    public int? ActivityId { get; set; }
    public int? GameObjectCode { get; set; }
    public int? GameObjectProgress { get; set; }
    public int? GameObjectLearningDurationInSec { get; set; }
}