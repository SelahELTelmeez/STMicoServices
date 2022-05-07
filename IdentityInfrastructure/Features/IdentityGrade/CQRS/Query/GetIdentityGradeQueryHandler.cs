﻿using IdentityDomain.Features.IdentityGrade.CQRS.Query;
using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ResultHandler;

namespace IdentityInfrastructure.Features.IdentityGrade.CQRS.Query;
public class GetIdentityGradeQueryHandler : IRequestHandler<GetIdentityGradeQuery, CommitResult<int>>
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
    public async Task<CommitResult<int>> Handle(GetIdentityGradeQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the user Id existance first, with the provided data.
        IdentityUser? identityUser = await _mediator.Send(new GetIdentityUserByIdQuery(_httpContextAccessor.GetIdentityUserId()), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult<int>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], // facebook data is Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }
        if (identityUser.IdentityRoleId != 1)
        {
            return new CommitResult<int>
            {
                ErrorCode = "X0007",
                ErrorMessage = _resourceJsonManager["X0007"], // only student can have grade.
                ResultType = ResultType.Invalid,
            };
        }
        return new CommitResult<int>
        {
            ResultType = ResultType.Ok,
            Value = identityUser.GradeId.GetValueOrDefault(),
        };
    }
}

