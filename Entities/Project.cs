using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.Entities;

public class Project
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string Title { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public int OwnerId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User Owner { get; set; } = null!;
    public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
} 