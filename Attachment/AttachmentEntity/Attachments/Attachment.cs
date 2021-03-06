using AttachmentEntities.Entities.Shared;

namespace AttachmentEntities.Entities.Attachments;

public class Attachment : GuidTrackableEntity
{
    public string Checksum { get; set; }
    public string Extension { get; set; }
    public string Name { get; set; }
    public string MineType { get; set; }
    public string FullName { get => $"{Name}{Extension}"; }
}
