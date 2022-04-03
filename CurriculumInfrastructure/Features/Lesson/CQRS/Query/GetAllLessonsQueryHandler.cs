using CurriculumDomain.Features.Lesson.CQRS.Query;
using CurriculumDomain.Features.Lesson.DTO.Query;
using CurriculumEntites.Entities;
using DomainEntitiesUnits = CurriculumEntites.Entities.Units;
using DomainEntitiesLessons = CurriculumEntites.Entities.Lessons;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CurriculumInfrastructure.Features.Lesson.CQRS.Query;
public class GetAllLessonsQueryHandler : IRequestHandler<GetAllLessonsQuery, List<LessonResponse>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetAllLessonsQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<List<LessonResponse>> Handle(GetAllLessonsQuery request, CancellationToken cancellationToken)
    {

        List<DomainEntitiesLessons.Lesson> Lessons = await _dbContext.Set<DomainEntitiesUnits.Unit>().Where(a => a.SubjectId == request.SubjectId)
                      .Include(a => a.Lessons).SelectMany(a => a.Lessons)
                      .ToListAsync(cancellationToken);

        return Lessons.Select(a => new LessonResponse
        {
            Id = a.Id,
            Name = a.Name,
            Ponits = a.Ponits,
            Clips = a.Clips.Adapt<List<ClipResponse>>(),
        }).ToList();

    }
}