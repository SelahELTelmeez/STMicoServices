using SharedModule.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentDomain.Features.Tracker.CQRS.Query;

    public record GetStudentQuizzResultQuery(Guid StudentId, int QuizId) : IRequest<ICommitResult<StudentQuizResultResponse>>;
    
   