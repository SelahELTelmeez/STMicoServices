using StudentEntities.Entities.Shared;

namespace StudentEntities.Entities.Rewards;
public class Reward : TrackableEntity
{
    public Guid StudentIdentityId { get; set; }
    public int Type { get; set; }
    public string SubjectId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsNew { get; set; }
    public MedalLevel MedalLevel { get; set; }
    public string Image { get; set; }
}