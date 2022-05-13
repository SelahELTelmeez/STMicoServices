using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModule.DTO
{
    public class StudentQuizResultRequest
    {
        public IEnumerable<int> QuizIds { get; set; }

        public Guid StudentId { get; set; }

    }
}
