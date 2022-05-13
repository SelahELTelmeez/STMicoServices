using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherDomain.Features.Quiz.DTO.Command;

namespace TeacherDomain.Features.Quiz.CQRS.Query;

    public record GetEnrolledStudentsByQuizActivityIdQuery(int ClassId, int QuizId) : IRequest<CommitResults<EnrolledStudentQuizResponse>>;
    
    

