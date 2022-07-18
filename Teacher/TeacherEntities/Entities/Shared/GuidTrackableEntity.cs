using System.ComponentModel.DataAnnotations;

namespace TeacherEntities.Entities.Shared;

public class GuidTrackableEntity
{
    [Key]
    public Guid Id { get; set; }
    public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
}
