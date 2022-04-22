using CurriculumDomain.Features.Subjects.GetSubjectUnits.DTO.Query;
using CurriculumDomain.Features.Subjects.GetTeacherSubjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetTeacherSubjects.DTO;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using Mapster;
using Microsoft.EntityFrameworkCore;
using DomainEntities = CurriculumEntites.Entities.Units;

namespace CurriculumInfrastructure.Features.Subjects.GetTeacherSubjects.CQRS.Query
{
    public class GetTeacjerSubjectsQueryHandler : IRequestHandler<GetTeacherSubjectsQuery, CommitResults<TeacherSubjectReponse>>
    {
        private readonly CurriculumDbContext _dbContext;

        public GetTeacjerSubjectsQueryHandler(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommitResults<TeacherSubjectReponse>> Handle(GetTeacherSubjectsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Subject> subjects = await _dbContext.Set<Subject>().Where(a => request.subjectIds.Contains(a.Id)).ToListAsync(cancellationToken);

            IEnumerable<UnitResponse> UnitResponses = await _dbContext.Set<DomainEntities.Unit>()
                                    .Where(a => request.subjectIds.Contains(a.SubjectId) && a.IsShow == true)
                                    .OrderBy(a => a.Sort)
                                    .Include(a => a.Lessons)
                                    .ProjectToType<UnitResponse>()
                                    .ToListAsync(cancellationToken);

            IEnumerable<TeacherSubjectReponse> Mapper()
            {
                foreach (Subject subject in subjects)
                {
                    yield return new TeacherSubjectReponse
                    {
                        SubjectId = subject.Id,
                        SubjectName = subject.ShortName,
                        Grade = subject.Grade,
                        PrimaryIcon = $"http://www.almoallem.com/media/LMSAPP/TeacherSubjectIcon/{subject.Id[..6]}.png",
                        InternalIcon = $"http://www.almoallem.com/media/LMSAPP/SubjectIcon/Icon/teacher/{subject.Title}.png",
                        TeacherGuide = $"https://www.selaheltelmeez.com/media2021/{subject.TeacherGuide}",
                        Term = subject.Term,
                        Units = UnitResponses.Where(a => a.SubjectId == subject.Id).ToList()
                    };
                }
                yield break;
            }

            return new CommitResults<TeacherSubjectReponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }
    }
}
