using System.ComponentModel.DataAnnotations;

namespace TransactionEntites.Entities.Shared;
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
