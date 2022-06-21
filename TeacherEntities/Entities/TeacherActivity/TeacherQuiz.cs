using TeacherEntities.Entities.Shared;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherEntities.Entities.TeacherActivity
{
    public class TeacherQuiz : TrackableEntity
    {
        public string Title { get; set; }
        public int ClipId { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid Creator { get; set; }
        public int QuizId { get; set; }
        public string? SubjectName { get; set; }
        public string? LessonName { get; set; }
        public virtual ICollection<TeacherClass> TeacherClasses { get; set; }
    }
}
