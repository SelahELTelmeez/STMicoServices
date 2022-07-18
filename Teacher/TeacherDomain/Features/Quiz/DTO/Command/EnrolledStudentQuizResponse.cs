using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Quiz.DTO.Command
{
    public class EnrolledStudentQuizResponse : EnrolledStudentResponse
    {
        public int? QuizScore { get; set; }
        public int? StudentScore { get; set; }
    }
}
