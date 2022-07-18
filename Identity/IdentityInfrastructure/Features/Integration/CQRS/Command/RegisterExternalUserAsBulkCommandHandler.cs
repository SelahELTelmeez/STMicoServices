using CsvHelper;
using CsvHelper.Configuration;
using Flaminco.CommitResult;
using IdentityDomain.Features.Integration.CQRS.Command;
using IdentityDomain.Features.Integration.DTO;
using System.Globalization;

namespace IdentityInfrastructure.Features.Integration.CQRS.Command;

public class RegisterExternalUserAsBulkCommandHandler : IRequestHandler<RegisterExternalUserAsBulkCommand, ICommitResult>
{
    private readonly IMediator _mediator;
    public RegisterExternalUserAsBulkCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task<ICommitResult> Handle(RegisterExternalUserAsBulkCommand request, CancellationToken cancellationToken)
    {

        using (var reader = new StreamReader(request.Stream))
        {
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<ExternalUserRegisterRequestMap>();

                await foreach (var item in csv.GetRecordsAsync<ExternalUserRegisterRequest>(cancellationToken))
                {
                    await _mediator.Send(new RegisterExternalUserCommand(item, request.ProviderSecretKey), cancellationToken);
                }
            }
        }

        return ResultType.Ok.GetCommitResult();

    }
}

public sealed class ExternalUserRegisterRequestMap : ClassMap<ExternalUserRegisterRequest>
{
    public ExternalUserRegisterRequestMap()
    {
        Map(x => x.ExternalUserId).Name("external_id");
        Map(x => x.RoleId).Name("role_id");
        Map(x => x.GradeId).Name("grade_id");
        Map(x => x.FullName).Name("full_name");
        Map(x => x.MobileNumber).Name("mobile_number");
    }
}
