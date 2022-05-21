using PaymentEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentEntities.Entities;

public class PurchaseContract : TrackableEntity
{
    public string PlanType { get; set; }
    public int ProductId { get; set; }
    public Guid UserId { get; set; }
    public string TransactionId { get; set; }
    public int TransactionStatus { get; set; }
    public DateTime ExpiredOn { get; set; }
    public string? Signature { get; set; }
    public DateTime? CallbackOn { get; set; }
    [ForeignKey(nameof(ProductId))] public Product ProductFK { get; set; }
}
