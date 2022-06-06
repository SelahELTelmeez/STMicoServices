namespace DashboardEntity.Entities;

public class AppSetting : BaseEntity
{
    public string Name { get; set; }
    public string Value { get; set; }
    public bool IsEnabled { get; set; }
}
