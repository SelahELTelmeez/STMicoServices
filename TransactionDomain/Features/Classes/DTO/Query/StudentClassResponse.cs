namespace TransactionDomain.Features.TeacherClass.DTO.Query
{
    public class StudentClassResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SubjectId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
