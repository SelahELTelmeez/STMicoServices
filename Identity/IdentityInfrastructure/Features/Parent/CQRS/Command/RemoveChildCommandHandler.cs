using Flaminco.CommitResult;
using IdentityDomain.Features.Parent.CQRS.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IdentityInfrastructure.Features.Parent.CQRS.Command;

public class RemoveChildCommandHandler : IRequestHandler<RemoveChildCommand, ICommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly string? _userId;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public RemoveChildCommandHandler(STIdentityDbContext dbContext,
                                     IWebHostEnvironment configuration,
                                     IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<ICommitResult> Handle(RemoveChildCommand request, CancellationToken cancellationToken)
    {
        // =========== Check for the relation of this student and parent existance first ================
        IdentityRelation? IdentityRelation = await _dbContext.Set<IdentityRelation>()
                                                             .FirstOrDefaultAsync(a =>
                                                             a.PrimaryId.Equals(_userId) && a.SecondaryId.Equals(request.ChildId)
                                                             && a.RelationType.Equals(RelationType.ParentToKid), cancellationToken);



        // =========== Remove child ================
        if (IdentityRelation != null)
        {
            _dbContext.Set<IdentityRelation>().Remove(IdentityRelation);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return ResultType.Ok.GetCommitResult();
        }
        // =========== Get Response ================
        return ResultType.NotFound.GetCommitResult("XIDN0019", _resourceJsonManager["XIDN0019"]);
    }
}