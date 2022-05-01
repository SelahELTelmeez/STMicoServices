using System.ComponentModel.DataAnnotations;

namespace PaymentEntities.Entities.Shared;
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
