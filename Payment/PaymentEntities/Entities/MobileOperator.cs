using PaymentEntities.Entities.Shared;

namespace PaymentEntities.Entities;

public class MobileOperator : BaseEntity
{
    public string Name { get; set; }
    public string MCC { get; set; }
    public string MNC { get; set; }
    public bool IsActive { get; set; }
}
