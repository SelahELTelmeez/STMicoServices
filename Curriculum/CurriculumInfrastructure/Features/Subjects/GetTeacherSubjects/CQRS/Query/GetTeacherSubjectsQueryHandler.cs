using CurriculumDomain.Features.Subjects.GetTeacherSubjects.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using CurriculumInfrastructure.HttpClients;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;
using DomainEntities = CurriculumEntites.Entities.Units;

namespace CurriculumInfrastructure.Features.Subjects.GetTeacherSubjects.CQRS.Query
{
    public class GetTeacherSubjectsQueryHandler : IRequestHandler<GetTeacherSubjectsQuery, CommitResults<TeacherSubjectResponse>>
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly IdentityClient _IdentityClient;
        public GetTeacherSubjectsQueryHandler(CurriculumDbContext dbContext, IdentityClient identityClient)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
        }

        public async Task<CommitResults<TeacherSubjectResponse>> Handle(GetTeacherSubjectsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Subject> subjects = await _dbContext.Set<Subject>()
                                                            .Where(a => request.subjectIds.Contains(a.Id))
                                                            .ToListAsync(cancellationToken);

            if (!subjects.Any())
            {
                return new CommitResults<TeacherSubjectResponse>
                {
                    ResultType = ResultType.Empty,
                    Value = Array.Empty<TeacherSubjectResponse>()
                };
            }

            CommitResults<GradeResponse>? grades = await _IdentityClient.GetGradesDetailesAsync(subjects.Select(a => a.Grade), cancellationToken);

            if (!grades.IsSuccess)
            {
                return new CommitResults<TeacherSubjectResponse>
                {
                    ErrorCode = grades.ErrorCode,
                    ErrorMessage = grades.ErrorMessage,
                    ResultType = grades.ResultType
                };
            }

            IEnumerable<DomainEntities.Unit> Units = await _dbContext.Set<DomainEntities.Unit>()
                                    .Where(a => request.subjectIds.Contains(a.SubjectId) && a.IsShow == true)
                                    .OrderBy(a => a.Sort)
                                    .Include(a => a.Lessons)
                                    .ProjectToType<DomainEntities.Unit>()
                                    .ToListAsync(cancellationToken);


            IEnumerable<UnitResponse> UnitMapper(string SubjectId)
            {
                foreach (DomainEntities.Unit unit in Units.Where(a => a.SubjectId == SubjectId))
                {
                    yield return new UnitResponse
                    {
                        Id = unit.Id,
                        Name = unit.ShortName,
                        SubjectId = unit.SubjectId,
                        Lessons = unit.Lessons.Select(a => new LessonResponse
                        {
                            Id = a.Id,
                            Name = a.ShortName
                        }),
                    };
                }
                yield break;
            }


            IEnumerable<TeacherSubjectResponse> Mapper()
            {
                foreach (Subject subject in subjects)
                {
                    GradeResponse? gradeResponse = grades.Value.FirstOrDefault(a => a.Id == subject.Grade);
                    yield return new TeacherSubjectResponse
                    {
                        SubjectId = subject.Id,
                        SubjectName = subject.ShortName,
                        Grade = subject.Grade,
                        GradeName = gradeResponse?.Name,
                        GradeShortName = gradeResponse?.ShortName,
                        PrimaryIcon = $"http://www.almoallem.com/media/LMSAPP/TeacherSubjectIcon/{subject.Id[..6]}.png",
                        InternalIcon = $"http://www.almoallem.com/media/LMSAPP/SubjectIcon/Icon/teacher/{subject.Title}.png",
                        TeacherGuide = $"https://www.selaheltelmeez.com/media2021/{subject.TeacherGuide}",
                        Term = subject.Term,
                        Units = UnitMapper(subject.Id)
                    };
                }
                yield break;
            }

            return new CommitResults<TeacherSubjectResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }

    }



}
