using System.ComponentModel.DataAnnotations.Schema;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.Shared;
using TeacherEntities.Entities.TeacherActivity;

namespace TeacherEntities.Entities.TeacherClasses;
public class TeacherClass : TrackableEntity
{
    public string Name { get; set; }
    public string SubjectId { get; set; }
    public string Description { get; set; }
    [Column(TypeName = "NVARCHAR(255)")]
    public Guid TeacherId { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; }
    public virtual ICollection<TeacherQuiz> TeacherQuizs { get; set; }
    public virtual ICollection<ClassEnrollee> ClassEnrollees { get; set; }
}
