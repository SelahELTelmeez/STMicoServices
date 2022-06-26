using IdentityDomain.Features.Shared.IdentityUser.CQRS.Query;
using IdentityDomain.Features.UpdateProfile.CQRS.Command;
using IdentityDomain.Features.UpdateProfile.DTO.Command;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Identities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ResultHandler;

namespace IdentityInfrastructure.Features.UpdateProfile.CQRS.Command;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, CommitResult>
{
    private readonly STIdentityDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMediator _mediator;

    public UpdateProfileCommandHandler(STIdentityDbContext dbContext,
                                       IWebHostEnvironment configuration,
                                       IHttpContextAccessor httpContextAccessor, IMediator mediator)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;
        _mediator = mediator;
    }
    public async Task<CommitResult> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {

        IdentityUser? identityUser = await _mediator.Send(new GetIdentityUserByIdQuery(_httpContextAccessor.GetIdentityUserId()), cancellationToken);

        if (identityUser == null)
        {
            return new CommitResult<UpdateProfileResponse>
            {
                ErrorCode = "XIDN0001",
                ErrorMessage = _resourceJsonManager["XIDN0001"], // facebook data is Exist, try to sign in instead.
                ResultType = ResultType.NotFound,
            };
        }

        // update values.
        if (request.UpdateProfile.AvatarId != null)
        {
            identityUser.AvatarId = request.UpdateProfile.AvatarId;
        }
        if (request.UpdateProfile.GradeId != null)
        {
            identityUser.GradeId = request.UpdateProfile.GradeId;
        }
        if (request.UpdateProfile.FullName != null)
        {
            identityUser.FullName = request.UpdateProfile.FullName;
        }
        if (request.UpdateProfile.GovernorateId != null)
        {
            identityUser.GovernorateId = request.UpdateProfile.GovernorateId;
        }
        if (request.UpdateProfile.CountryId != null)
        {
            identityUser.Country = (Country)request.UpdateProfile.CountryId.GetValueOrDefault();
        }
        if (request.UpdateProfile.DateOfBirth != null)
        {
            identityUser.DateOfBirth = request.UpdateProfile.DateOfBirth != null ? DateTime.Parse(request.UpdateProfile.DateOfBirth) : null;
        }
        if (request.UpdateProfile.Gender != null)
        {
            identityUser.Gender = (Gender)request.UpdateProfile.Gender.GetValueOrDefault();
        }
        _dbContext.Set<IdentityUser>().Update(identityUser);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}


