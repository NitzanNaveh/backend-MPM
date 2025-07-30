using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.Dtos.Tasks;

public class CreateTaskDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public DateTime? DueDate { get; set; }
    
    [Required]
    public int ProjectId { get; set; }
} 