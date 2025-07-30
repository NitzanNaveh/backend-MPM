using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.Dtos.Tasks;

public class UpdateTaskDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public DateTime? DueDate { get; set; }
    
    public bool IsCompleted { get; set; }
} 