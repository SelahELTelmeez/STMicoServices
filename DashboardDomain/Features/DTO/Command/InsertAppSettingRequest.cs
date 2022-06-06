namespace DashboardDomain.Features.DTO.Command
{
    public class InsertAppSettingRequest
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string IsEnabled { get; set; }
    }
}
