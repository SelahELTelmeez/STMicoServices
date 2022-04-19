using IdentityDomain.Features.IdentityUserTransaction.CQRS.Query;
using IdentityDomain.Features.IdentityUserTransaction.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityUserTransaction.CQRS.Query
{
    public class SearchOnStudentQueryHandler : IRequestHandler<SearchOnStudentQuery, CommitResult<SearchOnStudentResponse>>
    {
        private readonly STIdentityDbContext _dbContext;

        public SearchOnStudentQueryHandler(STIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CommitResult<SearchOnStudentResponse>> Handle(SearchOnStudentQuery request, CancellationToken cancellationToken)
        {
            return new CommitResult<SearchOnStudentResponse>
            {
                ResultType = ResultType.Ok,
                Value = await _dbContext.Set<IdentityUser>()
                                          .Where(a => String.IsNullOrEmpty(a.Email) ? a.MobileNumber.Equals(request.SearchOnStudentRequest.MobileNumber) : a.Email.Equals(request.SearchOnStudentRequest.Email))
                                          .Select(a => new SearchOnStudentResponse
                                          {
                                              Id = a.Id,
                                              FullName = a.FullName,
                                              GradeName = a.GradeFK.Name,
                                              AvatarImage = a.AvatarFK.ImageUrl
                                          })
                                          .FirstOrDefaultAsync(cancellationToken)
            };
        }
    }
}
