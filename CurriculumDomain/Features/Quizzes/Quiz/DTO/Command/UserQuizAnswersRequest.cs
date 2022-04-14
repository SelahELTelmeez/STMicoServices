using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurriculumDomain.Features.Quizzes.Quiz.DTO.Command
{
    public class UserQuizAnswersRequest
    {
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public int UserAnswersId { get; set; }
    }
}
