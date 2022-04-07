﻿using CurriculumDomain.Features.Quizzes.Quiz.DTO.Command;
using ResultHandler;

namespace CurriculumDomain.Features.Quizzes.Quiz.CQRS.Command;
public record CreateQuizCommand(QuizRequest QuizRequest) : IRequest<CommitResult<int>>;