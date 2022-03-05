using System.ComponentModel.DataAnnotations;

namespace CurriculumEntites
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
