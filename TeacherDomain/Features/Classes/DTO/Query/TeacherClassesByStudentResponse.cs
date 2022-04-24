using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherDomain.Features.Classes.DTO.Query
{
    public class TeacherClassesByStudentResponse
    {
        public string TeacherName { get; set; }
        public IEnumerable<SubjectBriefResponse> Subjects { get; set; }
        public IEnumerable<ClassBriefResponse> Classes { get; set; }
        public int ClassCounter { get; set; }
    }
}
