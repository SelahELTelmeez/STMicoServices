using TeacherEntities.Entities.Shared;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherEntities.Entities.TeacherActivity
{
    public class TeacherAssignment : TrackableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EndDate { get; set; }
        public Guid Creator { get; set; }
        public Guid AttachmentId { get; set; }
        public int AssignmentId { get; set; }
        public virtual ICollection<TeacherClass> TeacherClasses { get; set; }
    }
}
