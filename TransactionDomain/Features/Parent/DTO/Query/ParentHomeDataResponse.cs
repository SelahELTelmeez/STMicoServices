using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionDomain.Features.Parent.DTO.Query
{
    public class ParentHomeDataResponse
    {
        public Subjects Subjects { get; set; }
        public Teachers Teachers { get; set; }
        public Classes Classes { get; set; }
        public string GradeName { get; set; }
        public int term { get; set; }
    }


    public class Subjects
    {
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectImage { get; set; }

    }

    public class Teachers
    {
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string TeacherImage { get; set; }
    }

    public class Classes
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public DateTime JoinedDate { get; set; }
        public string SubjectName { get; set; }
    }
}