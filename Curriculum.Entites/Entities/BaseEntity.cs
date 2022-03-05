using System.ComponentModel.DataAnnotations;

namespace CurriculumEntites.Entities;
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
