using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.Entities;

public class TaskItem
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public required string Title { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    [Required]
    public bool IsCompleted { get; set; } = false;
    
    public int ProjectId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Project Project { get; set; } = null!;
} 