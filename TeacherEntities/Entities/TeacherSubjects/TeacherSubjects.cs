using TeacherEntities.Entities.Shared;

namespace TeacherEntities.Entities.TeacherSubjects;

public class TeacherSubject : TrackableEntity
{
    public Guid TeacherId { get; set; }
    public string SubjectId { get; set; }
    public bool IsDeleted { get; set; }
}
