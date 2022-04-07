using TransactionDomain.Features.Activities.DTO.Command;

namespace TransactionDomain.Features.Tracker.DTO.Command;

public class UpdateActivityRequest : InsertActivityRequest
{
    public int StudentPoints { get; set; } 
    public int LearningDurationInSec { get; set; } 
    public int Code { get; set; }  
    public int ActivityId { get; set; } 
}
