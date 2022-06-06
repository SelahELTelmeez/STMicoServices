using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardEntity.Entities;

public class Section : BaseEntity
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public string Thumbnail { get; set; }
    public string? YouTubeId { get; set; }
    public int SectionGroupId { get; set; }
    [ForeignKey(nameof(SectionGroupId))] public SectionGroup SectionGroupFK { get; set; }
}
