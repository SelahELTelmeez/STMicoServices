namespace ParentDomain.Features.Parent.DTO.Query;
    public class ClassesEntrolledByStudentResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public DateTime JoinedDate { get; set; }
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string GradeName { get; set; }
        public int term { get; set; }
    }