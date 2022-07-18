namespace DashboardDomain.Features.DTO.Query
{
    public class SectionResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string? Description { get; set; }
        public string Thumbnail { get; set; }
        public string? Url { get; set; }
    }
}
