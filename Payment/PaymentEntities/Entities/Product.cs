using PaymentEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentEntities.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string DescriptionAR { get; set; }
    public string DescriptionEN { get; set; }
    public decimal Price { get; set; }
    public int? Grade { get; set; }
    public int SubscriptionDurationInDays { get; set; }
    public int? PromotionId { get; set; }
    [ForeignKey(nameof(PromotionId))] public Promotion? PromotionFK { get; set; }

}
