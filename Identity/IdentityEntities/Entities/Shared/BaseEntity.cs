using System.ComponentModel.DataAnnotations;

namespace IdentityEntities.Entities.Shared;
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
