using TransactionDomain.Features.Activities.DTO.Command;

namespace TransactionDomain.Features.Tracker.DTO.Command;

public class UpdateActivityRequest : InsertActivityRequest
{
    public int StudentPoints { get; set; } // =0 in insert
    public int LearningDurationInSec { get; set; } // =0 in insert
    public int Code { get; set; }  // =0 in insert
                                   // public int Progress { get; set; }  // =0 in insert
    public int ActivityId { get; set; } //inh
}
