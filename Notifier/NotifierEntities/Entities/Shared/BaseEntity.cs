using System.ComponentModel.DataAnnotations;

namespace NotifierEntities.Entities.Shared;
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
