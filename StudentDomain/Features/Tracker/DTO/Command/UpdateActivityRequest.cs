namespace StudentDomain.Features.Tracker.DTO.Command;
public class UpdateActivityRequest
{
    public double StudentPoints { get; set; } // =0 in insert
    public int LearningDurationInSec { get; set; } // =0 in insert
    public int Code { get; set; }  // =0 in insert
    public int ActivityId { get; set; } //inh
}
