namespace TransactionEntites.Entities.Trackers
{
    public class TeacherAssignmentTracker
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EndDate { get; set; }
        public Guid Creator { get; set; }
        public string Attachment { get; set; }
        public int AssignmentId { get; set; }
    }
}
