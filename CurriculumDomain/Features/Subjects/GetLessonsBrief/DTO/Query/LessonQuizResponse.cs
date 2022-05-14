using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurriculumDomain.Features.Subjects.GetLessonsBrief.DTO.Query
{
    public class LessonQuizResponse
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public int QuizClipId { get; set; }
    }
}
