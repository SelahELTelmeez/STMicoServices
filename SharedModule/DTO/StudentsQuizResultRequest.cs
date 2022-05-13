using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.DTO
{
    public class StudentsQuizResultRequest
    {
        public IEnumerable<int> QuizIds { get; set; }

        public IEnumerable<Guid> StudentIds { get; set; }
    }
}
