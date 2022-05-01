using PaymentEntities.Entities.Shared;

namespace PaymentEntities.Entities;

public class Promotion : BaseEntity
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string DescriptionAR { get; set; }
    public string DescriptionEN { get; set; }
    public virtual ICollection<Promocode> Promocodes { get; set; }
}
