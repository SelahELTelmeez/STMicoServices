namespace TransactionDomain.Features.Activities.DTO.Command
{
    public class ActivityRequestDTO
    {
        public Guid StudentId { get; set; }
        public int StudentPoints { get; set; }
        public int LearningDurationInSec { get; set; }
        public bool IsActive { get; set; }
        public int Code { get; set; }
        public int progress { get; set; }

        public int ClipId { get; set; }
        public int LessonId { get; set; }

        public int ActivityId { get; set; }

    }
}
