using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query
{
    public class CheckAnyClassExistenceBySubjectIdQueryHandler : IRequestHandler<CheckAnyClassExistenceBySubjectIdQuery, ICommitResult<bool>>
    {
        private readonly TeacherDbContext _dbContext;
        public CheckAnyClassExistenceBySubjectIdQueryHandler(TeacherDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ICommitResult<bool>> Handle(CheckAnyClassExistenceBySubjectIdQuery request, CancellationToken cancellationToken)
        {
            return ResultType.Ok.GetValueCommitResult(await _dbContext.Set<TeacherClass>().AnyAsync(a => a.SubjectId.Equals(request.SubjectId)));
        }
    }
}
