using System.ComponentModel.DataAnnotations;

namespace TeacherEntities.Entities.Shared;
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
