using PaymentEntities.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentEntities.Entities;

public class Promocode : BaseEntity
{
    public string Code { get; set; }
    [Column(TypeName = "nvarchar(255)")]
    public string? IdentityId { get; set; }
    public bool? IsUsed { get; set; }
    public int PromotionId { get; set; }
    [ForeignKey(nameof(PromotionId))] public Promotion PromotionFK { get; set; }
}
