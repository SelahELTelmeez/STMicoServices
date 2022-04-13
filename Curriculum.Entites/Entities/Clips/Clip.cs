using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Clips;
public class Clip : BaseEntity
{
    public int? Sort { get; set; }
    public ClipType? Type { get; set; }
    public string? Title { get; set; }
    public string? FileName { get; set; }
    public int PageNo { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public ClipStatus? Status { get; set; }
    public string? Code { get; set; }
    public string? KNLDBank { get; set; }
    public string? KeyWords { get; set; }
    public bool? IsMedu { get; set; }
    public int? Usability { get; set; }
    public int? Points { get; set; }
    public Orientation? Orientation { get; set; }
    public bool? IsPremium { get; set; }
    public int? LessonId { get; set; }
    [ForeignKey(nameof(LessonId))] public Lesson? LessonFK { get; set; }
}