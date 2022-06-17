﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherDomain.Features.Quiz.CQRS.Query;
using TeacherDomain.Features.Quiz.DTO.Query;
using TeacherEntities.Entities.TeacherActivity;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Quiz.CQRS.Query
{
    public class GetQuizByIdQueryHandler : IRequestHandler<GetQuizByIdQuery,ICommitResult<QuizResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public GetQuizByIdQueryHandler(TeacherDbContext dbContext,
                                      IHttpContextAccessor httpContextAccessor,
                                      IWebHostEnvironment configuration)

        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }

        public async Task<ICommitResult<QuizResponse>> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
        {
            TeacherQuiz? teacherQuiz = await _dbContext.Set<TeacherQuiz>()
                                                          .Where(a => a.Id.Equals(request.Id))
                                                          .SingleOrDefaultAsync(cancellationToken);
            if(teacherQuiz == null)
            {
                return ResultType.NotFound.GetValueCommitResult<QuizResponse>(default, "X0016", _resourceJsonManager["X0016"]);
            }

            TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>()
                                                        .SingleOrDefaultAsync(a => a.Id == request.ClassId, cancellationToken);

            return ResultType.Ok.GetValueCommitResult(new QuizResponse
            {
                Description = teacherQuiz.Description,
                CreatedOn = teacherQuiz.StartDate,
                EndDate = teacherQuiz.EndDate,
                Id = teacherQuiz.Id,
                GetQuizDetailesId = teacherQuiz.QuizId,
                Title = teacherQuiz.Title,
                EnrolledCounter =0,
                SubjectName = teacherQuiz.SubjectName,
                LessonName = teacherQuiz.LessonName,
                ClassName = teacherClass?.Name ?? string.Empty
            });
        }
    }
}
