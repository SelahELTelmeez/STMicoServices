namespace TransactionDomain.Features.Assignment.DTO
{
    public class CreateAssignmentRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AttachmentUrl { get; set; }
        public List<int> Classes { get; set; }
    }
}
