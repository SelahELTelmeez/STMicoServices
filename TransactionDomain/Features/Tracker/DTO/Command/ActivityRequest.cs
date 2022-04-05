namespace TransactionDomain.Features.Activities.DTO.Command
{
    public class ActivityRequest
    {
        public int StudentPoints { get; set; }
        public int LearningDurationInSec { get; set; }
        public int Code { get; set; }
        public int Progress { get; set; }
        public int ClipId { get; set; }
        public int LessonId { get; set; }
    }
}
