using TeacherEntities.Entities.Shared;

namespace TeacherEntities.Entities.TeacherSubjects;

public class TeacherSubject : TrackableEntity
{
    public string TeacherId { get; set; }
    public string SubjectId { get; set; }
    public bool IsDeleted { get; set; }
}
