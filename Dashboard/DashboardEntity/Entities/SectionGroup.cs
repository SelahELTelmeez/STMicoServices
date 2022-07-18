namespace DashboardEntity.Entities
{
    public class SectionGroup : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Section> Sections { get; set; }
    }
}
