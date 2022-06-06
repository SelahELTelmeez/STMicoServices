namespace DashboardDomain.Features.DTO.Query;

public class SectionGroupResponse
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public IEnumerable<SectionResponse> Sections { get; set; }
}
