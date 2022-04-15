using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.TeacherSubjects;

public class TeacherSubject : TrackableEntity
{
    public Guid TeacherId { get; set; }
    public string SubjectId { get; set; }
}
