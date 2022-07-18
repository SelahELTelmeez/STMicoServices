using System.ComponentModel.DataAnnotations;

namespace CurriculumEntites.Entities.Shared;
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
