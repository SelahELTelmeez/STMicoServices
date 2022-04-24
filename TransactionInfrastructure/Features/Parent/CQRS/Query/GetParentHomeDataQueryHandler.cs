using TransactionDomain.Features.Parent.CQRS.Query;
using TransactionDomain.Features.Parent.DTO.Query;
using TransactionInfrastructure.HttpClients;

namespace TransactionInfrastructure.Features.Parent.CQRS.Query;

public class GetParentHomeDataQueryHandler : IRequestHandler<GetParentHomeDataQuery, CommitResults<ClassesEntrolledByStudentResponse>>
{
    //private readonly Guid? _userId;
    private readonly TeacherClient _TeacherClient;

    public GetParentHomeDataQueryHandler(TeacherClient TeacherClient)
    {
        _TeacherClient = TeacherClient;
    }
    public async Task<CommitResults<ClassesEntrolledByStudentResponse>> Handle(GetParentHomeDataQuery request, CancellationToken cancellationToken)
    {
        //CommitResults<ClassesEntrolledByStudentResponse>? ParentHomeData = await _TeacherClient.GetClassesEntrolledByStudentAsync(request.StudentId, cancellationToken);
        //if (ParentHomeData == null)
            return new CommitResults<ClassesEntrolledByStudentResponse>
            {
                ResultType = ResultType.NotFound
            };

        //return new CommitResults<ClassesEntrolledByStudentResponse>()
        //{
        //    ResultType = ResultType.Ok,
        //    Value = ParentHomeData.Value
        //};
    }
}