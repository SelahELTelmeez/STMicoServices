using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.TeacherClasses;

public class TeacherClass : TrackableEntity
{
    public string Name { get; set; }
    public string SubjectId { get; set; }
    public string Description { get; set; }
    public Guid TeacherId { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<StudentEnrollClass> StudentEnrolls { get; set; }
}
