namespace TransactionDomain.Features.Classes.DTO.Query
{
    public class EnrolledStudentResponse
    {
        public int ClassId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime JoinedDate { get; set; }
        public int GradeValue { get; set; }
        public string GradeName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
