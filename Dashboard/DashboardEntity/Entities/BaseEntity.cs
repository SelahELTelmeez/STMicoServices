using System.ComponentModel.DataAnnotations;

namespace DashboardEntity.Entities;
public class BaseEntity
{
    [Key]
    public int Id { get; set; }
}
