using CurriculumDomain.Features.Quizzes.Quiz.DTO.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurriculumDomain.Features.Quizzes.Quiz.CQRS.Command;

public record UserQuizAnswerCommand(UserQuizAnswersRequest UserQuizAnswersRequest) : IRequest<CommitResult<int>>;
