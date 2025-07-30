using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Api.Dtos.Projects;

public class CreateProjectDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
} 