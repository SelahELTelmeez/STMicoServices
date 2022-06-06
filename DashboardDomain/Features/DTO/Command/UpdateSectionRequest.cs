namespace DashboardDomain.Features.DTO.Command;

public class UpdateSectionRequest
{
    public int Id { get; set; }
    public int SectionGroup { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public string Thumbnail { get; set; }
    public string? YouTubeId { get; set; }
}
