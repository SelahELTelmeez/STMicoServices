using System.ComponentModel.DataAnnotations.Schema;
using TeacherEntities.Entities.Shared;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherEntites.Entities.TeacherClasses;
//LMSClassStudents
public class ClassEnrollee : TrackableEntity
{
    public int ClassId { get; set; }
    public string StudentId { get; set; }
    public bool IsActive { get; set; }
    [ForeignKey(nameof(ClassId))] public TeacherClass TeacherClassFK { get; set; }
}

/*
 * 0=> New
 * 1=> Seen
 * 3=> Opened
 * 4=> Finished 
 */