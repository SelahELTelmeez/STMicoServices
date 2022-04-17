using TransactionEntites.Entities.Shared;
using TransactionEntites.Entities.TeacherClasses;

namespace TransactionEntites.Entities.TeacherActivity
{
    public class TeacherAssignment : TrackableEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EndDate { get; set; }
        public Guid Creator { get; set; }
        public string AttachmentUrl { get; set; }
        public int AssignmentId { get; set; }
        public virtual ICollection<TeacherClass> TeacherClasses { get; set; }
    }
}
