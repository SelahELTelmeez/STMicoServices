using System.ComponentModel.DataAnnotations;

namespace StudentEntities.Entities.Shared;
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
