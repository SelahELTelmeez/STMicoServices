using System.ComponentModel.DataAnnotations.Schema;
using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.TeacherClasses;

public class StudentEnrollClass : TrackableEntity
{
    public int ClassId { get; set; }
    public Guid StudentId { get; set; }
    public bool IsActive { get; set; }
    [ForeignKey(nameof(ClassId))] public TeacherClass TeacherClassFK { get; set; }
}
