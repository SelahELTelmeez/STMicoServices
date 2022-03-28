namespace TransactionEntites.Entities.Shared;

public class TrackableEntity : BaseEntity
{
    public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
}
