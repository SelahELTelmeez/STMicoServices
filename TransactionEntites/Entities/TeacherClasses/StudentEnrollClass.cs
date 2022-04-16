using System.ComponentModel.DataAnnotations.Schema;
using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.TeacherClasses;

//LMSClassStudents
public class StudentEnrollClass : TrackableEntity
{
    public int ClassId { get; set; }
    public Guid StudentId { get; set; }
    public bool IsActive { get; set; }
    [ForeignKey(nameof(ClassId))] public TeacherClass TeacherClassFK { get; set; }
}

/*
 * 0=> New
 * 1=> Seen
 * 3=> Opened
 * 4=> Finished 
 */