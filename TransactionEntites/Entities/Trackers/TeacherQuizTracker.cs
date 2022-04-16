using TransactionEntites.Entities.Shared;

namespace TransactionEntites.Entities.Trackers
{
    //LMSActivities
    public class TeacherQuizTracker : TrackableEntity
    {
        public string Title { get; set; }
        public int ClipId { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid Creator { get; set; }
        public int QuizId { get; set; }
    }
}
