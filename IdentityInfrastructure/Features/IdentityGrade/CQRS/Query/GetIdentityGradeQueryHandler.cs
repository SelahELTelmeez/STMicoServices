using Flaminco.CommitResult;
using IdentityDomain.Features.IdentityGrade.CQRS.Query;
using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace IdentityInfrastructure.Features.IdentityGrade.CQRS.Query;
public class GetIdentityGradeQueryHandler : IRequestHandler<GetIdentityGradeQuery, ICommitResult<int>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IMediator _mediator;

    public GetIdentityGradeQueryHandler(IWebHostEnvironment configuration,
                                        IHttpContextAccessor httpContextAccessor,
                                        IMediator mediator)
    {
        _httpContextAccessor = httpContextAccessor;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _mediator = mediator;
    }
    public async Task<ICommitResult<int>> Handle(GetIdentityGradeQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _mediator.Send(new GetIdentityUserByIdQuery(request.IdentityId ?? _httpContextAccessor.GetIdentityUserId()), cancellationToken);

        if (identityUser == null)
        {
            return ResultType.NotFound.GetValueCommitResult<int>(default, "XIDN0001", _resourceJsonManager["XIDN0001"]);
        }
        if (identityUser.IdentityRoleId != 1)
        {
            return ResultType.Invalid.GetValueCommitResult<int>(default, "XIDN0007", _resourceJsonManager["XIDN0007"]);
        }
        return ResultType.Ok.GetValueCommitResult(identityUser.GradeId.GetValueOrDefault());
    }
}

