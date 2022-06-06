namespace DashboardDomain.Features.DTO.Command;

public class UpdateAppSettingRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public bool IsEnabled { get; set; }
}
